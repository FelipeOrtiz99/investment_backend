using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using InvestmentBackend.Domain.Services;

namespace InvestmentBackend.Infrastructure.Services;

public class SESEmailService : IEmailService
{
    private readonly IAmazonSimpleEmailService _sesClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SESEmailService> _logger;
    private readonly string _fromEmail;
    private readonly string _configurationSetName;

    public SESEmailService(
        IAmazonSimpleEmailService sesClient, 
        IConfiguration configuration,
        ILogger<SESEmailService> logger)
    {
        _sesClient = sesClient;
        _configuration = configuration;
        _logger = logger;
        _fromEmail = _configuration["AWS:SES:FromEmail"] ?? "noreply@example.com";
        _configurationSetName = _configuration["AWS:SES:ConfigurationSet"] ?? "";
    }

    public async Task SendTransactionNotificationAsync(string clientEmail, string clientName, TransactionEmailData transactionData)
    {
        try
        {
            var subject = $"Transaction Confirmation - {transactionData.TransactionType}";
            var htmlBody = GenerateTransactionEmailHtml(clientName, transactionData);
            var textBody = GenerateTransactionEmailText(clientName, transactionData);

            await SendEmailAsync(clientEmail, subject, htmlBody, textBody);
            
            _logger.LogInformation("Transaction notification sent to {Email} for transaction {TransactionId}", 
                clientEmail, transactionData.TransactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send transaction notification to {Email}", clientEmail);
            throw;
        }
    }

    public async Task SendInvestmentConfirmationAsync(string clientEmail, string clientName, InvestmentEmailData investmentData)
    {
        try
        {
            var subject = $"Investment Confirmation - {investmentData.InvestmentFundName}";
            var htmlBody = GenerateInvestmentEmailHtml(clientName, investmentData);
            var textBody = GenerateInvestmentEmailText(clientName, investmentData);

            await SendEmailAsync(clientEmail, subject, htmlBody, textBody);
            
            _logger.LogInformation("Investment confirmation sent to {Email} for investment {TransactionId}", 
                clientEmail, investmentData.TransactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send investment confirmation to {Email}", clientEmail);
            throw;
        }
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody, string textBody)
    {
        var request = new SendEmailRequest
        {
            Source = _fromEmail,
            Destination = new Destination
            {
                ToAddresses = new List<string> { toEmail }
            },
            Message = new Message
            {
                Subject = new Content(subject),
                Body = new Body
                {
                    Html = new Content(htmlBody),
                    Text = new Content(textBody)
                }
            }
        };

        if (!string.IsNullOrEmpty(_configurationSetName))
        {
            request.ConfigurationSetName = _configurationSetName;
        }

        var response = await _sesClient.SendEmailAsync(request);
        
        _logger.LogInformation("Email sent successfully. MessageId: {MessageId}", response.MessageId);
    }

    private string GenerateTransactionEmailHtml(string clientName, TransactionEmailData data)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Transaction Confirmation</title>
</head>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        <h1 style='color: #2c3e50; text-align: center; margin-bottom: 30px;'>Transaction Confirmation</h1>
        
        <p style='font-size: 16px; color: #333;'>Hello <strong>{clientName}</strong>,</p>
        
        <p style='font-size: 14px; color: #666; margin-bottom: 25px;'>
            Your transaction has been processed successfully. Here are the details:
        </p>
        
        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
            <table style='width: 100%; border-collapse: collapse;'>
                <tr>
                    <td style='padding: 8px 0; font-weight: bold; color: #2c3e50;'>Transaction ID:</td>
                    <td style='padding: 8px 0; color: #333;'>{data.TransactionId}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 0; font-weight: bold; color: #2c3e50;'>Type:</td>
                    <td style='padding: 8px 0; color: #333;'>{data.TransactionType}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 0; font-weight: bold; color: #2c3e50;'>Amount:</td>
                    <td style='padding: 8px 0; color: #333; font-size: 18px; font-weight: bold;'>{data.Amount:C} {data.Currency}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 0; font-weight: bold; color: #2c3e50;'>Date:</td>
                    <td style='padding: 8px 0; color: #333;'>{data.Date:yyyy-MM-dd HH:mm:ss} UTC</td>
                </tr>
                <tr>
                    <td style='padding: 8px 0; font-weight: bold; color: #2c3e50;'>Description:</td>
                    <td style='padding: 8px 0; color: #333;'>{data.Description}</td>
                </tr>
            </table>
        </div>
        
        <p style='font-size: 14px; color: #666; margin-top: 30px;'>
            Thank you for using our investment platform!
        </p>
        
        <hr style='border: none; height: 1px; background-color: #eee; margin: 30px 0;'>
        
        <p style='font-size: 12px; color: #999; text-align: center;'>
            This is an automated email. Please do not reply to this message.
        </p>
    </div>
</body>
</html>";
    }

    private string GenerateInvestmentEmailHtml(string clientName, InvestmentEmailData data)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Investment Confirmation</title>
</head>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        <h1 style='color: #27ae60; text-align: center; margin-bottom: 30px;'>🎉 Investment Confirmation</h1>
        
        <p style='font-size: 16px; color: #333;'>Congratulations <strong>{clientName}</strong>!</p>
        
        <p style='font-size: 14px; color: #666; margin-bottom: 25px;'>
            Your investment has been successfully processed. Here are the details:
        </p>
        
        <div style='background-color: #e8f5e8; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #27ae60;'>
            <table style='width: 100%; border-collapse: collapse;'>
                <tr>
                    <td style='padding: 8px 0; font-weight: bold; color: #2c3e50;'>Investment Fund:</td>
                    <td style='padding: 8px 0; color: #333; font-weight: bold;'>{data.InvestmentFundName}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 0; font-weight: bold; color: #2c3e50;'>Amount Invested:</td>
                    <td style='padding: 8px 0; color: #27ae60; font-size: 20px; font-weight: bold;'>{data.Amount:C} {data.Currency}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 0; font-weight: bold; color: #2c3e50;'>Transaction ID:</td>
                    <td style='padding: 8px 0; color: #333;'>{data.TransactionId}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 0; font-weight: bold; color: #2c3e50;'>Date:</td>
                    <td style='padding: 8px 0; color: #333;'>{data.Date:yyyy-MM-dd HH:mm:ss} UTC</td>
                </tr>
                <tr>
                    <td style='padding: 8px 0; font-weight: bold; color: #2c3e50;'>Minimum Investment:</td>
                    <td style='padding: 8px 0; color: #666;'>{data.MinimumInvestment:C} {data.Currency}</td>
                </tr>
            </table>
        </div>
        
        <div style='background-color: #fff3cd; padding: 15px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #ffc107;'>
            <p style='margin: 0; font-size: 14px; color: #856404;'>
                <strong>Important:</strong> Your investment will start generating returns according to the fund's performance. 
                You can track your investment progress in your dashboard.
            </p>
        </div>
        
        <p style='font-size: 14px; color: #666; margin-top: 30px;'>
            Thank you for choosing our investment platform. We're excited to help you grow your wealth!
        </p>
        
        <hr style='border: none; height: 1px; background-color: #eee; margin: 30px 0;'>
        
        <p style='font-size: 12px; color: #999; text-align: center;'>
            This is an automated email. Please do not reply to this message.
        </p>
    </div>
</body>
</html>";
    }

    private string GenerateTransactionEmailText(string clientName, TransactionEmailData data)
    {
        return $@"
Transaction Confirmation

Hello {clientName},

Your transaction has been processed successfully. Here are the details:

Transaction ID: {data.TransactionId}
Type: {data.TransactionType}
Amount: {data.Amount:C} {data.Currency}
Date: {data.Date:yyyy-MM-dd HH:mm:ss} UTC
Description: {data.Description}

Thank you for using our investment platform!

---
This is an automated email. Please do not reply to this message.
";
    }

    private string GenerateInvestmentEmailText(string clientName, InvestmentEmailData data)
    {
        return $@"
Investment Confirmation

Congratulations {clientName}!

Your investment has been successfully processed. Here are the details:

Investment Fund: {data.InvestmentFundName}
Amount Invested: {data.Amount:C} {data.Currency}
Transaction ID: {data.TransactionId}
Date: {data.Date:yyyy-MM-dd HH:mm:ss} UTC
Minimum Investment: {data.MinimumInvestment:C} {data.Currency}

IMPORTANT: Your investment will start generating returns according to the fund's performance. 
You can track your investment progress in your dashboard.

Thank you for choosing our investment platform. We're excited to help you grow your wealth!

---
This is an automated email. Please do not reply to this message.
";
    }
}
