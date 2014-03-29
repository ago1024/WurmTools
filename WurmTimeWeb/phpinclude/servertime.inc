<?php


function loadServerData($url) {
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

	if (!$response) {
		http_response_code(500);
		exit("Failed to retrieve server data");
	}

	list($header, $body) = explode("\r\n\r\n", $response, 2);

	$lastModified = "";
	$headers = explode("\r\n", $header);
	array_shift($headers);
	foreach ($headers as $line) {
		list($key, $value) = explode(": ", $line, 2);
		if ($key == "Last-Modified")
			$lastModified = $value;
	}

	return array($body, $lastModified);
}

function readServerTime($url) {


	$xml = new DOMDocument();

	list($html, $lastModified) = loadServerData($url);
	$xml->loadHTML($html);
	//$xml->loadHTMLFile("stats.jenn001.html");
	//$lastModified = "";

	$xpath = new DOMXpath($xml);
	$elements = $xpath->query("/html/body/table/tr[@class='statsTR' and ./td/text() = 'Wurm Time']/td[@class='statsDataTDValue']");
	if (!is_null($elements)) {
		foreach($elements as $element) {
			$timestamp = wurmtime_parse($element->textContent);
			if ($timestamp) {
				$timestamp["real"] = $lastModified;
				$timestamp["server"] = $element->textContent;
				return $timestamp;
			}
			return;
		}
	}
	http_response_code(500);
	exit("Failed to retrieve time in server data");
}

?>
