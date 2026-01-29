# Use .NET 8.0 SDK for development environment
FROM mcr.microsoft.com/dotnet/sdk:8.0

# Install any additional development tools
RUN apt-get update && apt-get install -y \
    git \
    curl \
    vim \
    && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /workspace

# Copy the entire workspace
COPY . .

# Restore dependencies for both projects
RUN dotnet restore CharacterGeneration/CharacterGeneration.csproj
RUN dotnet restore CharacterCreatationTest/CharacterCreatationTest.csproj

# Default to bash shell for development
CMD ["/bin/bash"]