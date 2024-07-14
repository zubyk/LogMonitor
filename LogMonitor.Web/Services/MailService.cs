using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Security.Authentication;
using System.Text;

namespace LogMonitor.Services
{
    public class MailService
    {

        private readonly ILogger<MailService> _logger;
        private readonly ApplicationConfigOptions _options;
        private readonly bool isOptionsCorrect;

        public MailService(ILogger<MailService> logger, IOptions<ApplicationConfigOptions> options)
        {
            _logger = logger;

            using (_logger.BeginScopeFromCaller())
            {
                _options = options.Value ?? throw new ArgumentNullException(nameof(options));

                if (!(isOptionsCorrect = Uri.CheckHostName(_options.SmtpHost) != UriHostNameType.Unknown))
                {
                    _logger.LogWarning("Invalid {SmtpHost}", _options.SmtpHost);
                }
                else if (!(isOptionsCorrect = _options.SmtpPort > 0))
                {
                    _logger.LogWarning("Invalid {SmtpPort}", _options.SmtpPort);
                }
                else if (!(isOptionsCorrect = CheckEmail(_options.SmtpFrom)))
                {
                    _logger.LogWarning("Invalid {SmtpFrom}", _options.SmtpFrom);
                }
                else if (!(isOptionsCorrect = _options.SmtpTo?.Any() != true))
                {
                    foreach (var email in _options.SmtpTo)
                    {
                        if (!(isOptionsCorrect &= CheckEmail(email)))
                        {
                            _logger.LogWarning("Invalid {SmtpTo}", email);
                        }
                    }
                }
            }
        }

        private static bool CheckEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            var hostName = email?.Split('@', StringSplitOptions.RemoveEmptyEntries)?.Skip(1)?.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(hostName)) return false;

            return Uri.CheckHostName(hostName) != UriHostNameType.Unknown;
        }

        public async Task SendReportAsync(string title, string filename, byte[] fileBody, string userEmail)
        {
            using (_logger.BeginScopeFromCaller())
            {
                _logger.LogMethodCall(args: new { title, filename, fileBody = $"byte[{fileBody?.Length}]" });

                if (!isOptionsCorrect)
                {
                    _logger.LogWarning($"Incorrect {nameof(ApplicationConfigOptions)} to send message");
                    return;
                }

                try
                {
                    using (var emailMessage = new MimeMessage())
                    {
                        foreach (var email in _options.SmtpTo?.Where(CheckEmail) ?? Enumerable.Empty<string>())
                        {
                            emailMessage.To.Add(new MailboxAddress("", email));
                        }

                        if (CheckEmail(userEmail))
                        {
                            if (!emailMessage.To.OfType<MailboxAddress>().Any(a => string.Equals(a.Address, userEmail, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                emailMessage.To.Add(new MailboxAddress("", userEmail));
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Invalid {SmtpTo}", userEmail);
                        }

                        if (emailMessage.To.Count == 0)
                            throw new Exception("Список адресов для рассылки отчета пустой!");

                        var builder = new BodyBuilder
                        {
                            TextBody = "Отчет сформирован автоматически"
                        };

                        builder.Attachments.Add(filename, fileBody);
                        emailMessage.Body = builder.ToMessageBody();

                        emailMessage.Subject = title;
                        emailMessage.Sender = new MailboxAddress(_options.MainTitle, _options.SmtpFrom);

                        using (var logStream = new MemoryStream())
                        using (var smtpLogger = new ProtocolLogger(logStream))
                        using (var client = new SmtpClient(smtpLogger))
                        {
                            try
                            {
                                client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                                client.SslProtocols = SslProtocols.Ssl2 | SslProtocols.Ssl3 | SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13;
                                await client.ConnectAsync(_options.SmtpHost, _options.SmtpPort ?? 0, SecureSocketOptions.Auto);

                                var opt = new FormatOptions()
                                {
                                    International = true
                                };

                                await client.SendAsync(opt, emailMessage);
                            }
                            finally
                            {
                                await client.DisconnectAsync(true);

                                if (logStream.Position > 0)
                                {
                                    logStream.Position = 0;
                                    _logger.LogDebug("{MailLog}", Encoding.UTF8.GetString(logStream.ToArray()));
                                }
                            }
                        }
                    }
                }
                catch (Exception e) when (_logger.FilterAndLogError(e))
                {
                    throw;
                }
            }
        }
    }
}
