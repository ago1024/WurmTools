<?php


$Starfalls_Diamonds = 0;
$Starfalls_Saw = 1;
$Starfalls_Digging = 2;
$Starfalls_Leaf = 3;
$Starfalls_Bears = 4;
$Starfalls_Snakes = 5;
$Starfalls_WhiteShark = 6;
$Starfalls_Fires = 7;
$Starfalls_Raven = 8;
$Starfalls_Dancers = 9;
$Starfalls_Omens = 10;
$Starfalls_Silence = 11;

$Seasons = Array(
Array( "name" => "Oleander", "starfall" => $Starfalls_Leaf, "week" => 2, "duration" => 1 ),
Array( "name" => "Maple", "starfall" => $Starfalls_Bears, "week" => 1, "duration" => 1 ),
Array( "name" => "Rose", "starfall" => $Starfalls_Bears, "week" => 1, "duration" => 5 ),
    //( "name" => "Rose", "starfall" => $Starfalls_Snakes, "week" => 1, "duration" => 1 ),
Array( "name" => "Lavender", "starfall" => $Starfalls_Bears, "week" => 1, "duration" => 1 ),
Array( "name" => "Camellia", "starfall" => $Starfalls_Bears, "week" => 1, "duration" => 2 ),
Array( "name" => "Cherry", "starfall" => $Starfalls_WhiteShark, "week" => 2, "duration" => 3 ),
Array( "name" => "Olive", "starfall" => $Starfalls_Fires, "week" => 2, "duration" => 3 ),
Array( "name" => "Olive", "starfall" => $Starfalls_Leaf, "week" => 2, "duration" => 1 ),
Array( "name" => "Grape", "starfall" => $Starfalls_Raven, "week" => 1, "duration" => 4 ),
Array( "name" => "Apple", "starfall" => $Starfalls_Raven, "week" => 1, "duration" => 4 ),
Array( "name" => "Lemon", "starfall" => $Starfalls_Omens, "week" => 2, "duration" => 3 ),
Array( "name" => "Walnut", "starfall" => $Starfalls_Omens, "week" => 2, "duration" => 3 ),
Array( "name" => "Chestnut", "starfall" => $Starfalls_Silence, "week" => 2, "duration" => 3 ),
);

/*
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

 */

function cmp($a, $b) 
{
	return $a["remaining"] - $b["remaining"];
}

function Seasons_getUpcoming($gametime)
{
	global $Seasons;
	$now = time();
	$upcoming = Array();
	$currentDay = $gametime["starfall"] * 28 + $gametime["day"];
	foreach ($Seasons as $season) {
		$seasonStart = $season["starfall"] * 28 + 7;
		$remaining = $seasonStart - $currentDay;

		$remaining += (86400 - $gametime["time"]) / 86400;
		if ($remaining < -28) {
			$remaining += 12 * 28;
		}
		$seconds = $remaining * 3 * 3600;
		$start = $now + $seconds;

		array_push($upcoming, 
			Array(
				"season" => $season,
				"remaining" => $remaining / 8,
				"start" => $start,
				"text" => date("r", $start),
				//"text" => Seasons_formatSpan($remaining / 8, "hours")
			));
	}
	usort($upcoming, "cmp");
	return $upcoming;
}

?>
