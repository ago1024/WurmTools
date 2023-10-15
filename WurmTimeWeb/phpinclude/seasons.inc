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
Array( "name" => "Olive", "starfall" => 7, "week" => 0, "duration" => 4 ),
Array( "name" => "Grape", "starfall" => 8, "week" => 0, "duration" => 4 ),
Array( "name" => "Cherry", "starfall" => 6, "week" => 0, "duration" => 4 ),
Array( "name" => "Apple", "starfall" => 8, "week" => 2, "duration" => 4 ),
Array( "name" => "Lemon", "starfall" => 8, "week" => 1, "duration" => 4 ),
Array( "name" => "Oleander", "starfall" => 3, "week" => 1, "duration" => 4 ),
Array( "name" => "Camellia", "starfall" => 3, "week" => 3, "duration" => 4 ),
Array( "name" => "Lavender", "starfall" => 4, "week" => 1, "duration" => 4 ),
Array( "name" => "Maple", "starfall" => 4, "week" => 3, "duration" => 4 ),
Array( "name" => "Rose", "starfall" => 4, "week" => 2, "duration" => 4 ),
Array( "name" => "Chestnut", "starfall" => 8, "week" => 3, "duration" => 4 ),
Array( "name" => "Walnut", "starfall" => 9, "week" => 1, "duration" => 4 ),
Array( "name" => "Pine", "starfall" => 0, "week" => 0, "duration" => 4 ),
Array( "name" => "Hazel", "starfall" => 9, "week" => 2, "duration" => 4 ),
Array( "name" => "Hops", "starfall" => 7, "week" => 2, "duration" => 4 ),
Array( "name" => "Oak", "starfall" => 5, "week" => 1, "duration" => 4 ),
Array( "name" => "Orange", "starfall" => 7, "week" => 3, "duration" => 4 ),
Array( "name" => "Raspberry", "starfall" => 9, "week" => 0, "duration" => 4 ),
Array( "name" => "Blueberry", "starfall" => 7, "week" => 1, "duration" => 4 ),
Array( "name" => "Lingonberry", "starfall" => 9, "week" => 3, "duration" => 4 ),
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
		$seasonStart = ($season["starfall"] * 28) + ($season["week"] * 7);
		$remaining = $seasonStart - $currentDay;

		$remaining += (86400 - $gametime["time"]) / 86400;
		if ($remaining < -28) {
			$remaining += 12 * 28;
		}
		$seconds = $remaining * 3 * 3600;
		$start = $now + $seconds;

		array_push($upcoming, 
			Array(
				"year" => $gametime["year"],
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
