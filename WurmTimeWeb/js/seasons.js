
function Seasons() {
}

Starfalls = WurmTime.Starfalls;
Seasons.seasons = [
{ "name" : "Olive", "starfall" : 7, "week" : 0, "duration" : 4 },
{ "name" : "Grape", "starfall" : 8, "week" : 0, "duration" : 4 },
{ "name" : "Cherry", "starfall" : 6, "week" : 0, "duration" : 4 },
{ "name" : "Apple", "starfall" : 8, "week" : 2, "duration" : 4 },
{ "name" : "Lemon", "starfall" : 8, "week" : 1, "duration" : 4 },
{ "name" : "Oleander", "starfall" : 3, "week" : 1, "duration" : 4 },
{ "name" : "Camellia", "starfall" : 3, "week" : 3, "duration" : 4 },
{ "name" : "Lavender", "starfall" : 4, "week" : 1, "duration" : 4 },
{ "name" : "Maple", "starfall" : 4, "week" : 3, "duration" : 4 },
{ "name" : "Rose", "starfall" : 4, "week" : 2, "duration" : 4 },
{ "name" : "Chestnut", "starfall" : 8, "week" : 3, "duration" : 4 },
{ "name" : "Walnut", "starfall" : 9, "week" : 1, "duration" : 4 },
{ "name" : "Pine", "starfall" : 0, "week" : 0, "duration" : 4 },
{ "name" : "Hazel", "starfall" : 9, "week" : 2, "duration" : 4 },
{ "name" : "Hops", "starfall" : 7, "week" : 2, "duration" : 4 },
{ "name" : "Oak", "starfall" : 5, "week" : 1, "duration" : 4 },
{ "name" : "Orange", "starfall" : 7, "week" : 3, "duration" : 4 },
{ "name" : "Raspberry", "starfall" : 9, "week" : 0, "duration" : 4 },
{ "name" : "Blueberry", "starfall" : 7, "week" : 1, "duration" : 4 },
{ "name" : "Lingonberry", "starfall" : 9, "week" : 3, "duration" : 4 },
];

Seasons.getCurrent = function(gametime)
{
    var current = [];
    for (var i = 0; i < Seasons.seasons.length; i++) {
        if (gametime.starfall == Seasons.seasons[i].starfall) {
            current.push(Seasons.seasons[i]);
        }
    }
    return current;
}

Seasons.truncate = function(val)
{
    if (val < 0)
        return Math.ceil(val);
    else
        return Math.floor(val);
}

Seasons.formatSpan = function(days, resolution)
{
    var seconds = days * 24 * 60 * 60;
    var minutes = days * 24 * 60;
    var hours = days * 24;
    var text = "";
    var filler = "";

    if (resolution == "days" && days < 1) {
        return "0 days";
    } else if (resolution == "hours" && hours < 1) {
        return "0 hours";
    } else if (resolution == "minutes" && minutes < 1) {
        return "0 minutes";
    } else if (seconds < 1) {
        return "0 seconds";
    }

    if (days >= 2) {
        var d = Seasons.truncate(days);
        text = d + " days";
        days -= d;
        filler = ", ";
    } else if (days >= 1) {
        text = "1 day";
        days -= 1;
        filler = ", ";
    }

    if (resolution == "days") {
        return text;
    }

    var hours = days * 24;
    if (hours >= 2) {
        text += filler + Seasons.truncate(hours) + " hours";
        hours -= Seasons.truncate(hours);
        filler = " ";
    } else if (hours >= 1) {
        text += filler + "1 hour";
        hours -= 1;
        filler = " ";
    }

    if (resolution == "hours") {
        return text;
    }

    var minutes = hours * 60;
    if (minutes >= 2) {
        text += filler + Seasons.truncate(minutes) + " minutes";
        minutes -= Seasons.truncate(minutes);
        filler = " ";
    } else if (minutes >= 1) {
        text += filler + "1 minute";
        minutes -= 1;
        filler = " ";
    }

    if (resolution == "minutes") {
        return text;
    }

    seconds = minutes * 60;
    if (seconds >= 2) {
        text += filler + Seasons.truncate(seconds) + " seconds";
        seconds -= Seasons.truncate(seconds);
        filler = " ";
    } else if (seconds >= 1) {
        text += filler + "1 second";
        seconds -= 1;
        filler = " ";
    }

    return text;
}

Seasons.getUpcoming = function(gametime)
{
    var now = new Date();
    var upcoming = [];
    var currentDay = gametime.starfall * 28 + gametime.day;
    for (var i = 0; i < Seasons.seasons.length; i++) {
        var season = Seasons.seasons[i];
        var seasonStart = season.starfall * 28 + season.week * 7;
        var remaining = seasonStart - currentDay;

        remaining += (86400 - gametime.time) / 86400;
        if (remaining < 0) {
            remaining += 12 * 28;
        }
        var seconds = remaining * 3 * 3600;
        var start = new Date(now.getTime() + seconds * 1000);

        upcoming.push({
                "season" : Seasons.seasons[i],
                "remaining": remaining / 8,
                "start": start,
                "text": Seasons.formatSpan(remaining / 8, "hours")
                });
    }
    upcoming.sort(function(a,b) { return a.remaining - b.remaining; });
    return upcoming;
}
