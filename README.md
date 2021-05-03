# MutualFundsStatementTracker
To automatically request mutual funds statement from RTA's website and send mail to aggregators (eg. Paytm, Groww, Piggy, Fisdom etc.), on a user defined frequency


Steps:
1) Open https://new.camsonline.com/Investors/Statements/Consolidated-Account-Statement
2) Fill up details: Statement Type, Period (Start date), With Zero balance folios, Email, PAN, Password. Submit details.
3) Get mail access rights (read and send). Read mails - Filter out mails with Subject "Consolidated Account Statement - CAMS Mailback Request". Pick latest mail (of same date or previous day).
4) Send to MF aggregators (eg. Paytm, Groww, Piggy, Fisdom etc.).

Inputs:
1) Frequency of sync (default: weekly). Scheduled Timings.
2) URL (default: https://new.camsonline.com/Investors/Statements/Consolidated-Account-Statement)
3) Details - PAN, Email, Password (default as PAN) etc
4) Email account - should match email in details. Error handling: account access and mismatch.
5) Subject to search on mail (default: "Consolidated Account Statement - CAMS Mailback Request"). Retry count (default: 5) and time (default: 10 min) for searching and sending mail.
6) List of Aggregator IDs (eg. upload@piggy.co.in)
