function WurmTime()
{
}

WurmTime.days = [ "Ant", "Luck", "Wurm", "Wrath", "Tears", "Sleep", "Awakening" ];
WurmTime.starfalls = [ "Diamonds", "Saw", "Digging", "Leaf", "Bears", "Snakes", "White Shark", "Fires", "Raven", "Dancers", "Omens", "Silence" ];

WurmTime.Starfalls = {};
WurmTime.Starfalls.Diamonds = 0;
WurmTime.Starfalls.Saw = 1;
WurmTime.Starfalls.Digging = 2;
WurmTime.Starfalls.Leaf = 3;
WurmTime.Starfalls.Bears = 4;
WurmTime.Starfalls.Snakes = 5;
WurmTime.Starfalls.WhiteShark = 6;
WurmTime.Starfalls.Fires = 7;
WurmTime.Starfalls.Raven = 8;
WurmTime.Starfalls.Dancers = 9;
WurmTime.Starfalls.Omens = 10;
WurmTime.Starfalls.Silence = 11;

WurmTime.Days = {};
WurmTime.Days.Ant = 0;
WurmTime.Days.Luck = 1;
WurmTime.Days.Wurm = 2;
WurmTime.Days.Wrath = 3;
WurmTime.Days.Tears = 4;
WurmTime.Days.Sleep = 5;
WurmTime.Days.Awakening = 6;


WurmTime.log = function(msg)
{
    document.write("<p>" + msg + "</p>");
}

WurmTime.pad = function(number)
{
    var r = String(number);
    if ( r.length === 1 ) {
        r = '0' + r;
    }
    return r;
}

WurmTime.getDayName = function(day)
{
    return WurmTime.days[day % 7];
}

WurmTime.getStarfallName = function(starfall)
{
    return WurmTime.starfalls[starfall % 12];
}

WurmTime.normalize = function(gametime)
{
    if (gametime.time < 0)
    {
        while (gametime.time < 0)
        {
            gametime.day--;
            gametime.time += 86400;
        }

        while (gametime.day < 0)
        {
            gametime.starfall--;
            gametime.day += 28;
        }

        while (gametime.starfall < 0)
        {
            gametime.year--;
            gametime.starfall += 12;
        }
    } else {
        while (gametime.time >= 86400)
        {
            gametime.day++;
            gametime.time -= 86400;
        }

        while (gametime.day >= 28)
        {
            gametime.starfall++;
            gametime.day -= 28;
        }

        while (gametime.starfall >= 12)
        {
            gametime.year++;
            gametime.starfall -= 12;
        }
    }

    gametime.hour = WurmTime.truncate(gametime.time / 3600) % 24;
    gametime.minute = WurmTime.truncate(gametime.time / 60) % 60;
    gametime.second = gametime.time % 60;
    gametime.text = WurmTime.pad(gametime.hour) + ":" + WurmTime.pad(gametime.minute) + ":" + WurmTime.pad(gametime.second) + " on " +
        WurmTime.getDayName(gametime.day) + " in week " + (WurmTime.truncate(gametime.day / 7) + 1) + " of the " + WurmTime.getStarfallName(gametime.starfall) + " starfall";

    return gametime;
}

WurmTime.truncate = function(value)
{
  if (value<0)
      return Math.ceil(value);
  else
      return Math.floor(value);
}


WurmTime.approximateTime = function(timestamp, reference)
{

    var difference = WurmTime.truncate((timestamp - new Date(reference.real)) / 1000);

    var gametime = {
        "time" : reference.wurm.time + difference * 8,
        "starfall" : reference.wurm.starfall,
        "day" : reference.wurm.day
    }

    return WurmTime.normalize(gametime);
}

WurmTime.parse = function(text)
{

            var days = [ "day of the Ant",
                              "Luck day",
                              "day of the Wurm",
                              "Wrath day",
                              "day of Tears",
                              "day of Sleep",
                              "day of Awakening" ];
            var starfalls = [ "the starfall of Diamonds",
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
                                   "the starfall of Silence" ];

            var TimeEntry = new RegExp('^It is (\\d\\d):(\\d\\d):(\\d\\d) on (.*) in week (\\d) of (.*) in the year of (\\d*).$');


            var match = TimeEntry.exec(text);
            if (match)
            {
                var day = days.indexOf(match[4]);
                var week = parseInt(match[5]);
                var starfall = starfalls.indexOf(match[6]);
                var year = parseInt(match[7]);

                var hour = parseInt(match[1]);
                var minute = parseInt(match[2]);
                var second = parseInt(match[3]);

                if (day == -1 || week == -1 || starfall == -1 || year == -1 || hour == -1 || minute == -1 || second == -1)
                {
                    document.logError("Parse Error " + match[0] + " " + JSON.stringify([day, week, starfall, year]));
                    return undefined;
                }

		return {
			"real" : new Date(),
			"wurm" : {
				"starfall" : starfall,
				"day" : day + 7 * week - 7,
				"hour" : hour,
				"minute" : minute,
				"second" : second,
				"time" : hour * 60 * 60 + minute * 60 + second,
				"text" : match[1] + ':' + match[2] + ':' + match[3] + ' on ' + WurmTime.days[day] + ' in week ' + week + ' of the ' + WurmTime.starfalls[starfall] + ' starfall'
			}
		}
	    }
	    return undefined;

}
