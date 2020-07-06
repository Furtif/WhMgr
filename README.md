# Brock Webhook Manager

### PokeAlarm alternative.  
Works with [RealDeviceMap](https://github.com/123FLO321/RealDeviceMap)  


## Description:  
Sends Discord notifications based on pre-defined filters for Pokemon, raids, raid eggs, field research quests, gym team changes, and weather. Also supports Discord user's subscribing to Pokemon, raid, quest, and Team Rocket invasion notifications via DM.


## Features:  
- Supports multiple Discord servers.  
- Discord channel alarm reports for Pokemon, raids, eggs, quests, lures, invasions, gym team changes, and weather.  
- Per user custom Discord notifications for Pokemon, raids, quests, and invasions.  
- User interface to configure Discord notifications with ease (as well as Discord commands). (https://github.com/versx/WhMgr-UI)  
- Notifications based on pre-defined distance.  
- Customizable alert messages with dynamic text replacement.  
- Support for multiple cities/areas using roles and geofences per server.  
- Daily shiny stats reporting.  
- Automatic quest message purge at midnight.  
- Support for Donors/Supporters only notifications.  
- Direct messages of Pokemon notifications based on city roles assigned.  
- Custom prefix support as well as mentionable user support for commands.  
- Subscriptions based on distance from a set location or specific gym names.  
- Twilio text message alerts for ultra rare Pokemon.  
- Custom image support for Discord alarm reports.  
- Custom icon style selection for Discord user notifications.  
- External emoji server support.  
- Custom static map format support.  
- Support for language translation.  
- Multi threaded, low processing consumption.  
- Lots more...  

## Documentation:  
[ReadTheDocs](https://whmgr.rtfd.io/)  

## Getting Started:  

1.) Run the following to install .NET Core runtime, clone respository, and copy example Alerts, Filters, Geofences, config and alarm files.  
```
Linux/macOS:  
wget https://raw.githubusercontent.com/versx/WhMgr/netcore/install.sh && chmod +x install.sh && ./install.sh && rm install.sh  

Windows:  
bitsadmin /transfer dotnet-install-job /download /priority FOREGROUND https://raw.githubusercontent.com/versx/WhMgr/netcore/install.bat install.bat | start install.bat  
```
2.) Edit `config.json` either open in Notepad/++ or `vi config.json`.  
  a.) [Create bot token](https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token)  
  b.) Input your bot token and config options.  
```js
{
    // Http listener port for raw webhook data.
    "port": 8008,
    // Locale language translation
    "locale": "en",
    // ShortURL API (yourls.org API, i.e. `https://domain.com/yourls-api.php?signature=XXXXXX`)
    "shortUrlApiUrl": null,
    // Stripe API key (Stripe production API key, i.e. rk_3824802934
    "stripeApiKey": ""
    // List of Discord servers to connect and post webhook messages to.
    "servers": {
        // Discord server #1 guild ID
        "000000000000000001": {
            // Bot command prefix, leave blank to use @mention <command>
            "commandPrefix": ".",
            // Discord server server ID.
            "guildId": 000000000000000001,
            // Discord Emoji server ID. (Can be same as `guildId`, currently not implemented, set as `guildId`)  
            "emojiGuildId": 000000000000000001,
            // Discord server owner ID.
            "ownerId": 000000000000000000,
            // Donor/Supporter role ID(s).
            "donorRoleIds": [
                000000000000000000
            ],
            // Moderator Discord ID(s).
            "moderatorIds": [
                000000000000000000
            ],
            // Discord bot token with user.
            "token": "<DISCORD_BOT_TOKEN>",
            // Alarms file path.
            "alarms": "alarms.json",
            // Enable custom direct message notification subscriptions.
            "enableSubscriptions": false,
            // Enable city role assignments.
            "enableCities": false,
            // City/geofence role(s)
            "cityRoles": [
                "City1",
                "City2"
            ],
            // Assigning city roles requires Donor/Supporter role.
            "citiesRequireSupporterRole": true,
            // Prune old field research quests at midnight.
            "pruneQuestChannels": true,
            // Channel ID(s) of quest channels to prune at midnight.
            "questChannelIds": [
                000000000000000000
            ],
            // Channel ID to post nests.
            "nestsChannelId": 000000000000000000,
            // Shiny stats configuration
            "shinyStats": {
                // Enable shiny stats posting.
                "enabled": true,
                // Clear previous shiny stat messages.
                "clearMessages": false,
                // Channel ID to post shiny stats.
                "channelId": 000000000000000000
            },
            // Icon style to use.
            "iconStyle": "Default",
            // Channel ID(s) bot commands can be executed in.
            "botChannelIds": [
                000000000000000000
            ],
            // Custom Discord status per server, leave blank or null to use current version.  
            "status": ""
        },
        "000000000000000002": {
            "commandPrefix": ".",
            "guildId": 000000000000000001,
            "emojiGuildId": 000000000000000001,
            "ownerId": 000000000000000000,
            "donorRoleIds": [
                000000000000000000
            ],
            "moderatorIds": [
                000000000000000000
            ],
            "token": "<DISCORD_BOT_TOKEN>",
            "alarms": "alarms2.json",
            "enableSubscriptions": false,
            "enableCities": false,
            "cityRoles": [
                "City3",
                "City4"
            ],
            "citiesRequireSupporterRole": true,
            "pruneQuestChannels": true,
            "questChannelIds": [
                000000000000000000
            ],
            "nestsChannelId": 000000000000000000,
            "shinyStats": {
                "enabled": true,
                "clearMessages": false,
                "channelId": 000000000000000000
            },
            "iconStyle": "Default",
            "botChannelIds": [
                000000000000000000
            ],
            "status": null
        }
    },
    // Database configuration
    "database": {
        // Database to store notification subscriptions.
        "main": {
            // Database hostname or IP address.
            "host": "127.0.0.1",
            // Database connection port.
            "port": 3306,
            // Database user account name.
            "username": "root",
            // Database user account password.
            "password": "password",
            // Brock database name.
            "database": "brock3"
        },
        // Scanner databse config
        "scanner": {
            // Database hostname or IP address.
            "host": "127.0.0.1",
            // Database connection port.
            "port": 3306,
            // Database user account name.
            "username": "root",
            // Database user account password.
            "password": "password",
            // RDM database name.
            "database": "rdmdb"
        },
        // PMSF Nests database config
        "nests": {
            // Database hostname or IP address.
            "host": "127.0.0.1",
            // Database connection port.
            "port": 3306,
            // Database user account name.
            "username": "root",
            // Database user account password.
            "password": "password",
            // PMSF nests database name.
            "database": "manualdb"
        }
    },
    // List of Pokemon IDs to treat as event and restrict postings and subscriptions to 90% IV or higher. (Filled in automatically with `event set` command)  
    "eventPokemonIds": [
        129,
        456,
        320
    ],
    // Image URL config
    "urls": {
        //Static tile map images template.
        "staticMap": "http://tiles.example.com:8080/static/klokantech-basic/{0}/{1}/15/300/175/1/png"
    },
    // Available icon styles
    "iconStyles": {
        "Default": "https://raw.githubusercontent.com/versx/WhMgr-Assets/master/original/",
        "Shuffle": "https://raw.githubusercontent.com/versx/WhMgr-Assets/master/shuffle/"
    },
    "staticMaps": {
        "pokemon": "pokemon.example.json",
        "raids": "raids.example.json",
        "quests": "quests.example.json",
        "invasions": "invasions.example.json",
        "lures": "lures.example.json",
        "gyms": "gyms.example.json",
        "nests": "nests.example.json",
        "weather": "weather.example.json"
    },
    // Get text message alerts with Twilio.com
    "twilio": {
        // Determines if text message alerts are be enabled
        "enabled": false,
        // Twilio account SID (Get via Twilio dashboard)
        "accountSid": "",
        // Twilio account auth token (Get via Twilio dashboard)
        "authToken": "",
        // Twilio phone number that will be sending the text message alert
        "from": "",
        // List of Discord user ids that can receive text message alerts
        "userIds": "",
        // List of acceptable Pokemon to receive text message alerts from
        "pokemonIds": [201, 480, 481, 482, 443, 444, 445, 633, 634, 635, 610, 611, 612],
        // Minimum acceptable IV value for Pokemon to be if not ultra rare (Unown, Lake Trio)
        "minIV": 100
    },
    // Log webhook payloads to a file for debugging
    "debug": false,
    // Only show logs with higher or equal priority levels
    "logLevel": "Trace"
}
```
3.) Edit `alarms.json` either open in Notepad/++ or `vi alarms.json`.  
4.) Fill out the alarms file.  
```js
{
    //Global switch for Pokemon notifications.
    "enablePokemon": false,
  
    //Global switch for Raid/Egg notifications.
    "enableRaids": false,
  
    //Global switch for Quest notifications.
    "enableQuests": false,
  
    //Global switch for Pokestop notifications.
    "enablePokestops": false,
  
    //Global switch for Gym notifications.
    "enableGyms": false,
    
    //Global switch for Weather notifications.
    "enableWeather": false,
  
    //List of alarms
    "alarms": [{
        //Alarm name.
        "name":"Alarm1",
      
        //Alerts file.
        "alerts":"default.json",
      
        //Alarm filters.
        "filters":"default.json",
      
        //Path to geofence file.
        "geofence":"geofence1.txt",
    
        //DTS compatible mention description.  
        "mentions":"<!@324234324> <iv> L<lvl> <geofence>"  
      
        //Discord webhook url address.
        "webhook":"<DISCORD_WEBHOOK_URL>"
    },{
        //Alarm name.
        "name":"Alarm2",
      
        //Alerts file.
        "alerts":"default.json",
      
        //Alarm filters.
        "filters":"100iv.json",
      
        //Path to geofence file.
        "geofence":"geofence1.txt",
    
        //DTS compatible mention description.  
        "mentions":""  
      
        //Discord webhook url address.
        "webhook":"<DISCORD_WEBHOOK_URL>"
    }]
}
```
5.) Create directory `geofences` in `bin/debug/netcoreapp2.1` directory if it doesn't already exist.  
6.) Create/copy geofence files to `geofences` folder.  

*Note:* Geofence file format is the following:  
```ini
[City1]
34.00,-117.00
34.01,-117.01
34.02,-117.02
34.03,-117.03
[City2]
33.00,-118.00
33.01,-118.01
33.02,-118.02
33.03,-118.03
```
7.) Add dotnet to your environment path if it isn't already (optional): `export PATH=~/.dotnet/dotnet:$PATH`  
8.) Build executable `dotnet build ../../..` (if dotnet is in your path) otherwise `~/.dotnet/dotnet build ../../..`  
9.) Start WhMgr `dotnet WhMgr.dll` (if dotnet is in your path) otherwise `~/.dotnet/dotnet WhMgr.dll` (If Windows, run as Administrator)  
10.) Optional User Interface for members to create subscriptions from a website instead of using Discord commands. (Still WIP but mostly done) [WhMgr UI](https://github.com/versx/WhMgr-UI)  

**Important Notes:**  
- Upon starting, database tables will be automatically created if `enableSubscriptions` is set to `true`. Emoji icons are also created upon connecting to Discord.  
- Discord Permissions Needed:  
  * Read Messages  
  * Send Messages  
  * Manage Messages (Prune quest channels)  
  * Manage Roles (If cities are enabled)  
  * Manage Emojis  
  * Embed Links  
  * Attach Files (`export` command)  
  * Use External Emojis  
- DM notifications can be sent to users based on:  
    - Pokemon ID  
    - Pokemon Form  
    - Pokemon IV  
    - Pokemon Level  
    - List of Pokemon Attack/Defense/Stamina values  
    - Pokemon Gender  
    - Raid Boss  
    - City  
    - Gym Name  
    - Quest Reward  
    - Invasion Grunt Type  
    - Distance (meters)  

## Notification Commands
**General Subscription Commands**  

* `enable` Enable direct message subscriptions  
* `disable` Disable direct message subscriptions  
* `info` List all Pokemon, Raid, Quest, Invasion, and Gym subscriptions and settings  
* `set-distance`  Set minimum distance to Pokemon, raids, quests, invasions and gyms need to be within. (Measured in meters)  
* `expire` / `expires` Check stripe API when Donor/Supporter subscription expires  

**Pokemon Subscriptions**  
* `pokeme` Subscribe to specific Pokemon notifications  
* `pokemenot` Unsubscribe from specific Pokemon notifications

**PVP Subscriptions**  
* `pvpme` Subscription to specific PVP Pokemon notifications
* `pvpmenot` Unsubscribe from specific PVP Pokemon notifications

**Raid Subscriptions**  
* `raidme` Subscribe to specific Raid notifications
* `raidmenot` Unsubscribe from specific Raid notifications

**Quest Subscriptions**  
* `questme` Subscribe to specific field research quest notifications
* `questmenot` Unsubscribe from specific field research quest notifications

**Team Rocket Invasion Subscriptions**  
* `invme` Subscribe to specific Team Rocket invasion notifications
* `invmenot` Unsubscribe from specific Team Rocket invasion notifications

**Subscriptions Management**  
* `import`  Import saved subscriptions file  
* `export`  Export subscriptions config file  

**Icon Style Selection**  
* `icons`  List available icon styles to choose from  
* `set-icons` Set icon style to use for direct message notifications  

**City Role Assignment**  
* `cities` / `feeds` List all available city roles  
* `feedme` Assign city role   
* `feedmenot` Unassign city role  

## Owner Only Commands  
* `gyms convert` Check for any pokestops that have converted to gyms and delete them from the database.  
* `nests` Post nests in channels.  
* `event list` List Pokemon set as event Pokemon  
* `event set <pokemon_id_list>` Set Pokemon as event Pokemon (overwrites current list)  
* `isbanned` Check if IP banned from PTC or NIA  
* `clean-departed` Clean departed Discord member subscriptions  
* `reset-quests` Reset and delete quest channels  
* `shiny-stats` Manually post shiny stats  

## Dynamic Text Replacement  
__**Pokemon**__  

| Place Holder | Description  | Example
|---|---|---|  
| pkmn_id  | Pokedex ID  |  1
| pkmn_id_3  | Pokedex ID (always 3 digits)  | 001
| pkmn_name | Pokemon name | Bulbasaur
| pkmn_img_url | Pokemon image url | http://example.com/your-specified-pokemon-url
| form_id | Form ID | 65
| form_id_3 | Form ID (always 3 digits) | 065
| costume | Costume name | Witch Hat
| costume_id | Costume ID | 835
| costume_id_3 | Costume ID (always 3 digits) | 835
| cp | Combat Power value | 1525
| lvl | Pokemon level | 25
| gender | Pokemon gender | Gender icon
| size | Pokemon size | Big
| move_1 | Fast move name | Quick Attack
| move_2 | Charge move name | Thunder
| moveset | Fast & Charge move names | Quick Attack/Thunder
| type_1 | Pokemon type | Dark
| type_2 | Pokemon type | Water
| types | Both types (if 2nd exists) | Dark/Fire
| types_emoji | Type Discord emoji | <:00000:types_fire> <00001:types_dark>
| atk_iv | Attack IV stat | 15
| def_iv | Defense IV stat | 7
| sta_iv | Stamina IV stat | 13
| iv | IV stat (including percent sign) | 100%
| iv_rnd | Rounded IV stat | 96%
| is_great | Great League stats (bool) | true
| is_ultra | Ultra League stats (bool) | false
| is_pvp | Has either Great or Ultra league stats | true
| pvp_stats | PvP stat ranking strings | 
| height | Pokemon height | 0.79
| weight | Pokemon weight | 116
| is_ditto | Checks if Ditto | true
| original_pkmn_id | Pokedex ID of Ditto disguise | 13
| original_pkmn_id_3 | Pokedex ID of Ditto disguise (always 3 digits) | 013
| original_pkmn_name | Pokemon name of Ditto diguise | Weedle
| is_weather_boosted | Returns if Pokemon is weather boosted | true
| has_weather | Returns if Pokemon data has weather | false
| weather | Weather in-game name | PartlyCloudy
| weather_emoji | Weather in-game emoji | Weather
| is_shiny | Returns if worker encountered a shiny | true
| username | Account username of account that found Pokemon | Frank0324
| spawnpoint_id | Spawnpoint ID Pokemon near | 3920849203840983204980
| encounter_id | Encounter ID of Pokemon | 392874987239487924
| despawn_time | Pokemon despawn time | 07:33:01 PM
| despawn_time_verified | Indicates if time is confirmed or not | `~` for not verified
| is_despawn_time_verified | Returns if despawn time is verified | true
| time_left | Minutes and seconds of time left until despawn | 29m, 30s
| geofence | Geofence name Pokemon is in | City1
| lat | Latitude coordinate of Pokemon location | 5.980921321
| lng | Longitude coordinate of Pokemon location | 3.109283009
| lat_5 | Latitude coordinate shortend to 5th precision | 5.98092
| lng_5 | Longitude coordinate shortend to 5th precision | 3.10928
| tilemaps_url | Static tile map url | http://tiles.example.com/static/pokemon-1.png
| gmaps_url | Google maps location url | https://maps.google.com/maps?q=5.980921321,3.109283009
| applemaps_url | Apple maps location url | https://maps.apple.com/maps?daddr=5.980921321,3.109283009
| wazemaps_url | Waze maps location url | https://www.waze.com/ul?ll=5.980921321,3.109283009&navigate=yes
| near_pokestop | Returns if Pokemon is near a Pokestop | true
| pokestop_id | Nearby Pokestop ID | 9382498723849792348798234.16
| pokestop_name | Name of nearby Pokestop | The Amazing Pokestop
| pokestop_url | Image url of nearby Pokestop | https://google.com/imgs/gym.png
| br | Newline break | `\r\n`

__**Raids & Eggs**__  

| Place Holder | Description  | Example
|---|---|---|  
| pkmn_id  | Raid boss pokedex ID  |  1
| pkmn_id_3  | Raid boss pokedex ID (always 3 digits)  | 001
| pkmn_name | Raid boss pokemon name | Bulbasaur
| pkmn_img_url | Raid boss pokemon image url | http://example.com/your-specified-pokemon-url
| form_id | Form ID | 65
| form_id_3 | Form ID (always 3 digits) | 065
| is_egg | Returns if raid is egg and not hatched | false
| is_ex | Returns if raid is ex pass eligible | true
| ex_emoji | Ex emoji icon | Ex
| team | Team name that has gym control | Valor
| team_emoji | Emoji of team that has gym control | <:valor:930824>
| cp | Raid boss combat power value | 36150
| lvl | Raid boss level | 5
| gender | Pokemon gender | Gender icon
| move_1 | Fast move name | Quick Attack
| move_2 | Charge move name | Thunder
| moveset | Fast & Charge move names | Quick Attack/Thunder
| type_1 | Pokemon type | Dark
| type_2 | Pokemon type | Water
| types | Both types (if 2nd exists) | Dark/Fire
| types_emoji | Type Discord emoji | <:00000:types_fire> <00001:types_dark>
| weaknesses | Raid boss weaknesses | Rock, Ground, Dark
| weaknesses_emoji | Emoji(s) of raid boss weaknesses | Rock Ground Dark
| perfect_cp | Perfect IV CP | 1831
| perfect_cp_boosted | Perfect IV CP if Weather boosted | 2351
| worst_cp | Worst IV CP | 1530 
| worst_cp_boosted | Worst IV CP if Weather boosted | 1339
| start_time | Raid start time | 08:32:00 AM
| start_time_left | Time left until raid starts | 43m, 33s
| end_time | Raid end time | 09:15:10 AM
| end_time_left | Time left until raid ends | 45, 11s
| geofence | Geofence name raid boss is in | City1
| lat | Latitude coordinate of Pokemon location | 5.980921321
| lng | Longitude coordinate of Pokemon location | 3.109283009
| lat_5 | Latitude coordinate shortend to 5th precision | 5.98092
| lng_5 | Longitude coordinate shortend to 5th precision | 3.10928
| tilemaps_url | Static tile map url | http://tiles.example.com/static/pokemon-1.png
| gmaps_url | Google maps location url | https://maps.google.com/maps?q=5.980921321,3.109283009
| applemaps_url | Apple maps location url | https://maps.apple.com/maps?daddr=5.980921321,3.109283009
| wazemaps_url | Waze maps location url | https://www.waze.com/ul?ll=5.980921321,3.109283009&navigate=yes
| gym_id | Gym ID | 9382498723849792348798234.16
| gym_name | Name of Gym | The Amazing Gym
| gym_url | Image url of Gym | https://google.com/imgs/gym.png
| br | Newline break | `\r\n`

__**Quests**__  

| Place Holder | Description  | Example
|---|---|---|  
| quest_task | Quest task message | Catch 5 Pokemon
| quest_conditions | Quest task conditions | Dark
| quest_reward | Quest task reward | Chansey
| quest_reward_img_url | Quest reward image url | http://map.example.com/images/quest.png
| has_quest_conditions | Returns if the quest has conditions | true
| is_ditto | Checks if Ditto | true
| is_shiny | Checks if reward is shiny | false
| geofence | Geofence name raid boss is in | City1
| lat | Latitude coordinate of Pokemon location | 5.980921321
| lng | Longitude coordinate of Pokemon location | 3.109283009
| lat_5 | Latitude coordinate shortend to 5th precision | 5.98092
| lng_5 | Longitude coordinate shortend to 5th precision | 3.10928
| tilemaps_url | Static tile map url | http://tiles.example.com/static/pokemon-1.png
| gmaps_url | Google maps location url | https://maps.google.com/maps?q=5.980921321,3.109283009
| applemaps_url | Apple maps location url | https://maps.apple.com/maps?daddr=5.980921321,3.109283009
| wazemaps_url | Waze maps location url | https://www.waze.com/ul?ll=5.980921321,3.109283009&navigate=yes
| pokestop_id | Pokestop ID | 9382498723849792348798234.16
| pokestop_name | Name of Pokestop | The Amazing Pokestop
| pokestop_url | Image url of Gym | https://google.com/imgs/gym.png
| br | Newline break | `\r\n`

**Pokestops**  

| Place Holder | Description  | Example
|---|---|---|  
| has_lure | Returns if Pokestop has active lure module deployed | true
| lure_type | Pokestop lure module type | Glacial
| lure_expire_time | Time lure module will expire | 07:33:19 PM
| lure_expire_time_left | Time left until lure module expires | 13m, 2s
| has_invasion | Returns if Pokestop has active Team Rocket invasion | false
| grunt_type | Grunt type | Water
| grunt_type_emoji | Emoji icon of grunt type | <:938294:types_water>
| grunt_gender | Grunt gender | Male
| invasion_expire_time | Time the invasion expires | 02:17:11 PM
| invasion_expire_time_left | Time left until invasion expires | 12m, 56s
| invasion_encounters | Possible invasions reward encounters | 80% Bulbasaur
| geofence | Geofence name raid boss is in | City1
| lat | Latitude coordinate of Pokemon location | 5.980921321
| lng | Longitude coordinate of Pokemon location | 3.109283009
| lat_5 | Latitude coordinate shortend to 5th precision | 5.98092
| lng_5 | Longitude coordinate shortend to 5th precision | 3.10928
| tilemaps_url | Static tile map url | http://tiles.example.com/static/pokemon-1.png
| gmaps_url | Google maps location url | https://maps.google.com/maps?q=5.980921321,3.109283009
| applemaps_url | Apple maps location url | https://maps.apple.com/maps?daddr=5.980921321,3.109283009
| wazemaps_url | Waze maps location url | https://www.waze.com/ul?ll=5.980921321,3.109283009&navigate=yes
| pokestop_id | Pokestop ID | 9382498723849792348798234.16
| pokestop_name | Name of Pokestop | The Amazing Pokestop
| pokestop_url | Image url of Gym | https://google.com/imgs/gym.png
| br | Newline break | `\r\n`

**Gyms**  

| Place Holder | Description  | Example
|---|---|---|  
| gym_id | Gym ID | 032840982304982034.16
| gym_name | Name of Gym | The Amazing Gym
| gym_url | Image url of Gym | https://google.com/imgs/gym.png
| gym_team | Current team that has gym control | Valor
| gym_team_emoji | Emoji icon of current team that has gym control | <:09833:valor>
| old_gym_team | Previous gym team that had gym control | Mystic
| old_gym_team_emoji | Emoji icon of previous gym team that has gym control | <:324987:mystic>
| team_changed | Returns if team's gym control changed | true
| in_battle | Returns if there's a current battle at the gym taking place | false
| under_attack | Returns if there's a current battle at the gym taking place | false
| is_ex | Returns if the gym is an ex raid eligible location | true
| ex_emoji | Ex emoji icon | <:809809:ex>
| slots_available | Number of available gym slots | 3
| geofence | Geofence name raid boss is in | City1
| lat | Latitude coordinate of Pokemon location | 5.980921321
| lng | Longitude coordinate of Pokemon location | 3.109283009
| lat_5 | Latitude coordinate shortend to 5th precision | 5.98092
| lng_5 | Longitude coordinate shortend to 5th precision | 3.10928
| tilemaps_url | Static tile map url | http://tiles.example.com/static/pokemon-1.png
| gmaps_url | Google maps location url | https://maps.google.com/maps?q=5.980921321,3.109283009
| applemaps_url | Apple maps location url | https://maps.apple.com/maps?daddr=5.980921321,3.109283009
| wazemaps_url | Waze maps location url | https://www.waze.com/ul?ll=5.980921321,3.109283009&navigate=yes
| br | Newline break | `\r\n`  

**Weather**  

| Place Holder | Description  | Example
|---|---|---|  
| id | Weather Cell ID | -932840982304982034
| weather_img_url | Image url of Weather condition type | https://google.com/imgs/weather_2.png
| weather_condition | Weather condition type string | Clear
| geofence | Geofence name raid boss is in | City1
| lat | Latitude coordinate of Pokemon location | 5.980921321
| lng | Longitude coordinate of Pokemon location | 3.109283009
| lat_5 | Latitude coordinate shortend to 5th precision | 5.98092
| lng_5 | Longitude coordinate shortend to 5th precision | 3.10928
| tilemaps_url | Static tile map url | http://tiles.example.com/static/pokemon-1.png
| gmaps_url | Google maps location url | https://maps.google.com/maps?q=5.980921321,3.109283009
| applemaps_url | Apple maps location url | https://maps.apple.com/maps?daddr=5.980921321,3.109283009
| wazemaps_url | Waze maps location url | https://www.waze.com/ul?ll=5.980921321,3.109283009&navigate=yes
| br | Newline break | `\r\n`  


## TODO:  
- Allow Pokemon id and name in Pokemon filter lists.  
- Individual filters per Pokemon. (PA style, maybe)  
- Reload config on change
- PvP ranks DTS
- Separate subscriptions DTS
- Wiki.  


## Examples:  
*All examples are completely customizable using Dynamic Text Replacement/Substitution*
Discord Pokemon Notifications:  
![Pokemon Notifications](images/pkmn.png "Pokemon Notifications")  

Discord Raid Notifications:  
![Raid Notifications](images/raid.png "Raid Notifications")  

Discord Raid Egg Notifications:  
![Egg Notifications](images/egg.png "Egg Notifications")  

Discord Quest Notifications:  
![Quest Notifications](images/quests.png "Quest Notifications")  

Discord Lure Notifications:  
![Lure Notifications](images/lure.png "Lure Notifications")  

Discord Lure (Glacial) Notifications:  
![Lure (Glacial) Notifications](images/lure_glacial.png "Lure (Glacial) Notifications")  

Discord Lure (Mossy) Notifications:  
![Lure (Mossy) Notifications](images/lure_mossy.png "Lure (Mossy) Notifications")  

Discord Lure (Magnetic) Notifications:  
![Lure (Magnetic) Notifications](images/lure_magnetic.png "Lure (Magnetic) Notifications")  

Discord Gym Team Takeover Notifications:  
![Gym Team Takeover Notifications](images/gyms.png "Gym Team Takeover Notifications")  

Discord Team Rocket Invasion Notifications:  
![Team Rocket Invasion Notifications](images/invasions.png "Team Rocket Invasion Notifications")  

## Current Issues:  
- Gender icon comes in as `?` so -m and -f are used for now.  

## Credits:  
[versx](https://github.com/versx) - Developer  
[PokeAlarm](https://github.com/PokeAlarm/PokeAlarm) - Dynamic Text Substitution idea  
[WDR](https://github.com/PartTimeJS/WDR) - masterfile.json file  

## Discord  
https://discordapp.com/invite/zZ9h9Xa  
