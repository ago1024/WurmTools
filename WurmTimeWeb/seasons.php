<?php
require('phpinclude/wurmtime.inc');
require('phpinclude/servertime.inc');
require('phpinclude/seasons.inc');
$url = 'http://jenn001.game.wurmonline.com/battles/stats.html';

date_default_timezone_set('UTC');
$timestamp = readServerTime($url);
if (!$timestamp) {
	http_response_code(500);
	exit(0);
}

header('Content-Type: text/calendar; charset="utf-8"');

$seasons = Seasons_getUpcoming($timestamp["wurm"]);

$dtformat = 'Ymd\THis\Z';
$now = time();
printf("BEGIN:VCALENDAR\n");
printf("PRODID:-//PHP//Version-Egal//EN\n");
printf("VERSION:2.0\n");
printf("X-WR-CALDESC:Upcoming wurm seasons\n");
printf("X-WR-CALNAME:Wurm seasons\n");
printf("\n");

foreach ($seasons as $season) {
	printf("BEGIN:VEVENT\n");
	printf("CREATED:%s\n", date($dtformat));
	printf("SUMMARY:%s\n", $season["season"]["name"]);
	printf("CLASS:PUBLIC\n");
	printf("DTSTART:%s\n", date($dtformat, $season["start"]));
	printf("DTEND:%s\n", date($dtformat, $season["start"] + (3*7*24*3600/8)));
	printf("URL:%s\n", 'http://gotti.no-ip.org/wurm/seasons');
	printf("END:VEVENT\n");
	printf("\n");
}
printf("END:VCALENDAR\n");




?>

