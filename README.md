# TelegramToRss

Read your favorite Telegram channels via RSS feed. No Telegram account needed.

## How it _will_ work

1. Sign-In/Sign Up (via e-mail or Google account).
2. Add Telegram channels to watch.
3. TelegramToRss will watch for new posts and save them to DB.
4. Subscribe to your private RSS feed using any RSS reader.

## TODO

* [x] Set up PostgreSQL and EF Core to store channels and posts
* [x] Set up RSS feed Web API
* [x] Configuration
* [x] Self-contained builds for Linux and Windows
* [x] Test on Ubuntu
* [ ] Continuous test (several days)
* [ ] Authorization (simple hardcoded login/password)
* [ ] Private RSS feed (w/ token)
* [ ] Configure RSS feed aging (drop old posts)
* [ ] Web UI: login, channel CRUD
* [ ] Google Sign-In
