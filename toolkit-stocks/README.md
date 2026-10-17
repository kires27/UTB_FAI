`python3 src/main.py --overview --balance --cash --income --dividends --prices --splits --statistics --fa`

## supabase libraries

pip install python-dotenv psycopg2

### system dependencies

sudo pacman -S postgresql-libs postgresql gcc

**Create a virtual environment:**
`python3 -m venv venv`

**activation**
`source ./.venv-kyos/bin/activate.fish`

`deactivate`

`pip list`

**get python requirements**
`pip freeze > requirements.txt`

# maybe
#todo
- name: Install PostgreSQL libs
  run: |
    sudo apt-get update
    sudo apt-get install -y libpq-dev