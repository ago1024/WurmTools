
function Seasons() {
}

Starfalls = WurmTime.Starfalls;
Seasons.seasons = [
{ "name" : "Oleander", "starfall" : Starfalls.Leaf, "week" : 2, "duration" : 1 },
{ "name" : "Maple", "starfall" : Starfalls.Bears, "week" : 1, "duration" : 1 },
{ "name" : "Rose", "starfall" : Starfalls.Bears, "week" : 1, "duration" : 5 },
    //{ "name" : "Rose", "starfall" : Starfalls.Snakes, "week" : 1, "duration" : 1 },
{ "name" : "Lavender", "starfall" : Starfalls.Bears, "week" : 1, "duration" : 1 },
{ "name" : "Camellia", "starfall" : Starfalls.Bears, "week" : 1, "duration" : 2 },
{ "name" : "Cherry", "starfall" : Starfalls.WhiteShark, "week" : 2, "duration" : 3 },
{ "name" : "Olive", "starfall" : Starfalls.Fires, "week" : 2, "duration" : 3 },
{ "name" : "Olive", "starfall" : Starfalls.Leaf, "week" : 2, "duration" : 1 },
{ "name" : "Grape", "starfall" : Starfalls.Raven, "week" : 1, "duration" : 4 },
{ "name" : "Apple", "starfall" : Starfalls.Raven, "week" : 1, "duration" : 4 },
{ "name" : "Lemon", "starfall" : Starfalls.Omens, "week" : 2, "duration" : 3 }
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
        var seasonStart = season.starfall * 28 + 7;
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
