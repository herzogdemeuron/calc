# Directus (Currently not up to date)

<span style="color:red">**WARNING:** If you are working on an existing Directus project be very careful to not delete your existing collections.    
**ALWAYS** spin up a local instance with your scheme and test first! The included `update-directus-schema.py` script uses the `force` argument to "bypass version and database vendor restrictions".</span>

## Test Setup

Follow the official [Quickstart Guide](https://docs.directus.io/self-hosted/quickstart.html) to get a self hosted version of diretus up and running.

> Remember to remove the `database` and `uploads` folders if you want a fresh start when re-composing the container.

## Apply CALC database schema

1. Open a terminal in calc/directus
2. Run `update-directus-schema.py` to add the required collections, fields and relations to a directus instance.

> Make sure to configure the email, password and all urls as needed.
