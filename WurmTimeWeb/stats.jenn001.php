<?php
// PHP Proxy
// Responds to both HTTP GET and POST requests
//
// Author: Abdul Qabiz
// March 31st, 2006
//

// Get the url of to be proxied
$url = 'http://jenn001.game.wurmonline.com/battles/stats.html';

//Start the Curl session
$session = curl_init($url);

// Don't return HTTP headers. Do return the contents of the call
curl_setopt($session, CURLOPT_HEADER, true);

//curl_setopt($session, CURLOPT_FOLLOWLOCATION, true);
//curl_setopt($ch, CURLOPT_TIMEOUT, 4);
curl_setopt($session, CURLOPT_RETURNTRANSFER, true);

// Make the call
$response = curl_exec($session);

curl_close($session);

list($header, $body) = explode("\r\n\r\n", $response, 2);

foreach (explode("\r\n", $header) as $line) {
	header($line);
}

echo $body;

?>

