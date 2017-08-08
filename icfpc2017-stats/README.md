# boilerplate-app

A boilerplate code to save time making a new Node.js app.

To make a new app:

* Clone this repo with `git clone https://github.com/igorlukanin/boilerplate-app.git <folder>`
* Install default packages: `npm install --save bluebird compression config ect express rethinkdb`
* Change files
* Run the app: `npm run website` — or better `supervisor -e js,ect -x npm -- run website`

## Files to be changed

* In `.git/config`: upstream URL
* In `package.json`: app name
* In `config/default.json`: `rethinkdb.db` and `website.port`
* In `db/index.js`: tables and indexes
* In `views/index.ect`: page title and content
* In `views/layout.ect`: mobile-friendly meta tags, favicons, and analytics counter

## Useful packages

Not included to dependencies in `package.json` by default:

* `cookie-parser`
* `lodash`
* `lru-cache`
* `moment` and `moment-timezone`
* `got` or `request`, sometimes `throttled-request`
* `uuid`
* `xlsx`
* `glob`
