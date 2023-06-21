# Unit Testing in Calc.Core

This repository contains the unit tests for Calc.Core. To run the unit tests successfully, you need to provide a configuration file with the necessary settings, including the API access token, specific to your testing environment.

## Configuration Setup

To configure the necessary settings for unit testing, follow these steps:

1. Create a new configuration file named `test.config` in the root directory of the unit test project.

2. Open `test.config` in a text editor and configure the required settings, such as the API access token, specific to your testing environment. Make sure to follow the appropriate structure and format for the configuration file.

   Example `test.config`:
   ```xml
   <?xml version="1.0" encoding="utf-8"?>
   <configuration>
     <appSettings>
       <add key="ApiAccessToken" value="your-api-access-token" />
       <!-- Add other necessary settings for unit testing -->
     </appSettings>
   </configuration>
   ``` 

3. Save the test.config file.

## Usage
When running the unit tests, ensure that the test runner picks up the test.config file and uses the specified configuration settings:

Build the unit test project to ensure that the test.config file is included in the output directory.

Run the unit tests using your preferred test runner (e.g., MSTest, NUnit). The test runner should automatically detect and use the configuration settings from the test.config file.

Note: If your test runner does not automatically pick up the test.config file, you may need to configure it manually to include the file in the test execution.

## Security Considerations
Please ensure that you do not commit the test.config file to the public repository. To prevent accidental commits, add test.config to the .gitignore file in the root directory of your local repository.