# Pricer

European option pricing application, which provide both analytical and numerical solutions. 
Additionally, program calculates $\Delta$ for every priced option.

It is a final assignment in QuantScholarship 2023 from Credit Suisse.

## Table of Contents

- [Installation and Usage](#installation)
- [Contributing](#contributing)

## Installation

To run this application, you will need to install the .NET SDK, which provides the necessary tools and libraries for building and running .NET applications. Below are the steps to install .NET and run the application:

### 1. Install .NET:

#### Windows:
1. Visit the [.NET downloads](https://dotnet.microsoft.com/en-us/download) page on the official Microsoft website.
2. Download the latest version of the .NET SDK for Windows.
3. Follow the installation instructions provided by the installer

#### Linux:

1. Open a terminal window.
2. Add the Microsoft package signing key and repository to your system by running the following commands:
```sh
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
```
Note: Replace "ubuntu/20.04" with your Linux distribution and version if you are not using Ubuntu 20.04.

3. Install the .NET SDK by running the following commands:
```sh
sudo apt update
sudo apt install -y apt-transport-https
sudo apt update
sudo apt install -y dotnet-sdk-6.0
```

#### macOS:

1. Open a terminal window.
2. Install Homebrew if you haven't already. You can install it by running the following command:

```sh
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
```

3. Use Homebrew to install the .NET SDK by running the following command:
```sh
brew install --cask dotnet-sdk
```
### 2. Verify Installation:
To ensure that the .NET SDK is installed correctly, open a terminal or command prompt and run the following command:
```sh
dotnet --version
```

### 3. Clone the Repository:
Clone the repository containing the application code using Git:

```sh
git clone https://github.com/BanasiakB/Pricer.git
```

### 4. Navigate to the Application Directory:

```sh
cd <repository_directory>
```

### 5. Build and Run the Application:

Once you are inside the application directory, use the following commands to build and run the application:

```sh
dotnet build
dotnet run
```
The dotnet build command compiles the application, and dotnet run command executes it.

### 6. Access the Application:
After successfully running the application, you can access it through a web browser.

That's it! You have successfully installed .NET and run the application. If you encounter any issues, please refer to the official .NET documentation or seek assistance from the community.

## Meta

Distributed under the MIT license. See `LICENSE` for more information.

[https://github.com/BanasiakB/](https://github.com/BanasiakB/)

## Contributing

1. Fork it (https://github.com/BanasiakB/Pricer.git)
2. Create your feature branch (`git checkout -b feature/fooBar`)
3. Commit your changes (`git commit -am 'Add some fooBar'`)
4. Push to the branch (`git push origin feature/fooBar`)
5. Create a new Pull Request
