# Calc Example Setup

**This guide helps you quickly set up Calc with a customized local Directus instance. If you're using a self-hosted or cloud-based Directus from scratch, ensure it has the [Calc Directus Schema](./calc_directus_schema.json) integrated. Calc requires this schema to function properly.**

## Step 1: Clone the Repository

1. Clone this repo to your local machine.
2. If you're new to Git, follow the [Visual Studio Code guide on Git repositories](https://code.visualstudio.com/docs/sourcecontrol/intro-to-git#_open-a-git-repository).

## Step 2: Set Up Directus with Docker

1. Ensure Docker is installed on your machine. If not, [download and install Docker](https://docs.docker.com/get-started/get-docker/).
2. Ensure docker is running in the background, consider add your account to the 'docker-users' group if docker doesn't boot.
3. Navigate to the **quick_start/directus** of the cloned folder in File Explorer.
4. Type `cmd` in the address bar and press Enter to open a command prompt.
5. Run the command `docker compose up`.
6. Wait for the message "**Server started at http://0.0.0.0:8055**" to appear.
7. Keep this terminal open.
8. If you have any questions, check out to the [Direcuts Official Guide](https://docs.directus.io/self-hosted/quickstart.html) 

## Step 3: Access and Verify Directus

1. Open [http://localhost:8055](http://localhost:8055) in your browser.
2. Log in with these credentials:
   - Username: **admin@example.com**
   - Password: **d1r3ctu5**
3. Check the left navigation bar for Calc collections. If visible, Directus is successfully set up.

## Step 4: Install Calc for Revit

- Run the **calc_revit_install.bat** file in the quick_start folder
- Wait for the success message. Once received, the plugin is ready to use.

## Step 5: Login to Calc

1. Open Revit and run the Calc App from the Add-In tab.
2. Log in using the URL **http://localhost:8055** and the same credentials as in Step 3.

## Uninstall

1. Ensure Revit is closed.
2. Run the **calc_revit_uninstall.bat** file

