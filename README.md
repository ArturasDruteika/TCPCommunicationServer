# TCPCommunicationServer

## Overview
TCPCommunicationServer is a C# based project designed for TCP communication. It works with a TCPCommunicationClient (https://github.com/ArturasDruteika/TCPCommunicationClient.git) repo as a server - client communication.

## Getting Started
### Prerequisites
- .NET Framework - __.NET 8.0__

### Installation
1. git clone https://github.com/ArturasDruteika/TCPCommunicationServer.git
2. cd path/to/TCPCommunicationServer
3. Open TCPCommunicationServer.sln
4. Run the server

## Usage
This server works with client (https://github.com/ArturasDruteika/TCPCommunicationClient). You have to pull that repo and run it according to it's README.md file.

## Features
- TCP connection handling with client management
- Asynchronous message handling
- Image transmission and handling
- Custom event handling for new and disconnected clients

## Usage
- Start the server using `ServerRunner`.
- Connect clients to the server.
- The server handles messages and images, broadcasting messages to other clients and saving received images.
