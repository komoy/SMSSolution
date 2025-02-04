# SMS Web API with Twilio Integration

This project is an ASP.NET Core Web API solution that demonstrates how to use a dedicated controller to handle both a REST endpoint for sending SMS messages and a webhook endpoint to process external events (such as new user signups) using Twilio’s API.

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [Testing the Endpoints](#testing-the-endpoints)
- [Project Structure](#project-structure)
- [License](#license)

## Features

- **REST API Endpoint:**  
  Provides a `/api/sms/send-sms` endpoint to manually trigger SMS messages.
  
- **Webhook Endpoint:**  
  Provides a `/api/sms/webhook` endpoint that listens for events (e.g., new signup) and sends SMS notifications.
  
- **Twilio Integration:**  
  Uses Twilio’s .NET SDK to send SMS messages.

- **Configuration via appsettings.json:**  
  Reads Twilio credentials from the configuration file.

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) (or later)
- A [Twilio](https://www.twilio.com/) account (free trial is sufficient)
- An IDE or editor (e.g., Visual Studio, VS Code)

## Installation

1. **Clone the repository:**

   ```bash
   git clone https://github.com/yourusername/SMSWebApi.git
   cd SMSWebApi
