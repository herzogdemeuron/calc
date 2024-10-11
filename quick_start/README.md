# Quick Start

**This guide helps you quickly set up Calc with a local Directus instance. If you're using a self-hosted or cloud-based Directus, ensure it has the [Calc Directus Schema](./quick_start/calc_directus_schema.json) integrated. Calc requires this schema to function properly.**

## Step 1: Clone the Repository

1. Clone this repo to your local machine.
2. If you're new to Git, follow the [Visual Studio Code guide on Git repositories](https://code.visualstudio.com/docs/sourcecontrol/intro-to-git#_open-a-git-repository).

## Step 2: Set Up Directus with Docker

1. Ensure Docker is installed on your machine. If not, [download and install Docker](https://docs.docker.com/get-started/get-docker/).
2. Navigate to the **directus** folder in File Explorer.
3. Type `cmd` in the address bar and press Enter to open a command prompt.
4. Run the command `docker compose up`.
5. Wait for the message "**Server started at http://0.0.0.0:8055**" to appear.
6. If you have any questions, refer to the [Direcuts Official Guide](https://docs.directus.io/self-hosted/quickstart.html) 

## Step 3: Access and Verify Directus

1. Open [http://localhost:8055](http://localhost:8055) in your browser.
2. Log in with these credentials:
   - Username: **admin@example.com**
   - Password: **d1r3ctu5**
3. Check the left navigation bar for Calc collections. If visible, Directus is successfully set up.
## Step 3: Install Calc for Revit
- 



<span style="color:red">**WARNING:** If you are working on an existing Directus project be very careful to not delete your existing collections.    
**ALWAYS** spin up a local instance with your scheme and test first! The included `update-directus-schema.py` script uses the `force` argument to "bypass version and database vendor restrictions".</span>

## Test Setup

Follow the official [Quickstart Guide](https://docs.directus.io/self-hosted/quickstart.html) to get a self hosted version of diretus up and running.

> Remember to remove the `database` and `uploads` folders if you want a fresh start when re-composing the container.

## Apply CALC database schema

1. Open a terminal in calc/directus
2. Run `update-directus-schema.py` to add the required collections, fields and relations to a directus instance.

Notes:

You might need to click around your test db for a while until you can create items.
You might need to redo all the relations between collections. To check if you need to do this, try to make an item in calc_forests. Check if you see an error regarding the project_id field.

> Make sure to configure the email, password and all urls as needed.
