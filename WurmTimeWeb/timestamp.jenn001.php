<?php
require('phpinclude/wurmtime.inc');
require('phpinclude/servertime.inc');
$url = 'http://jenn001.game.wurmonline.com/battles/stats.html';
$timestamp = readServerTime($url);
if ($timestamp)
	echo json_encode($timestamp);
?>

