<?php

function wurmtime_parse($text)
{

	$wurmtime_days = Array( "Ant", "Luck", "Wurm", "Wrath", "Tears", "Sleep", "Awakening" );
	$wurmtime_starfalls = Array( "Diamonds", "Saw", "Digging", "Leaf", "Bears", "Snakes", "White Shark", "Fires", "Raven", "Dancers", "Omens", "Silence" );

	$days = Array( "day of the Ant",
		"Luck day",
		"day of the Wurm",
		"Wrath day",
		"day of Tears",
		"day of Sleep",
		"day of Awakening" );
	$starfalls = Array( "the starfall of Diamonds",
		"the starfall of the Saw",
		"the starfall of the Digging",
		"the starfall of the Leaf",
		"the Bear's starfall",
		"the Snake's starfall",
		"the White Shark starfall",
		"the starfall of Fires",
		"the Raven's starfall",
		"the starfall of Dancers",
		"the starfall of Omens",
		"the starfall of Silence" );

	$match = array();

	if (preg_match('/^It is (\\d\\d):(\\d\\d):(\\d\\d) on (.*) in week (\\d) of (.*) in the year of (\\d*).$/', $text, $match))
	{
		$day = array_search($match[4], $days);
		$week = intval($match[5]);
		$starfall = array_search($match[6], $starfalls);
		$year = intval($match[7]);

		$hour = intval($match[1]);
		$minute = intval($match[2]);
		$second = intval($match[3]);

		if ($day === false || $week == 0 || $starfall === false || $year == 0)
		{
			http_response_code(500);
			exit("Parse Error: " . implode(" " , Array($match[0], $day, $week, $starfall, $year)));
			return false;
		}

		return array(
			"real" =>  "",
			"wurm" => array(
				"starfall" => $starfall,
				"day" => $day + 7 * $week - 7,
				"hour" => $hour,
				"minute" => $minute,
				"second" => $second,
				"time" => $hour * 60 * 60 + $minute * 60 + $second,
				"text" => $match[1] . ':' . $match[2] . ':' . $match[3] . ' on ' . $wurmtime_days[$day] . ' in week ' . $week . ' of the ' . $wurmtime_starfalls[$starfall] . ' starfall'
			)
		);
	}
	http_response_code(500);
	exit("Parse Error: " . $text);
	return false;

}
?>
