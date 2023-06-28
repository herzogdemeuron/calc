# Directus

## Test Setup

Follow the official [Quickstart Guide](https://docs.directus.io/self-hosted/quickstart.html) to get a self hosted version of diretus up and running.

> Remember to remove the `database` and `uploads` folders if you want a fresh start when re-composing the container.

## Apply CALC database schema

1. Open a terminal in calc/directus
2. Run `get-directus-schema.py` to add the required collections, fields and what not to a directus instance.

> Make sure to configure the email, password and all urls as needed.