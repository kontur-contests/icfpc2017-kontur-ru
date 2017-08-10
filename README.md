# ICFPC 2017. Team kontur.ru

## Key ideas

As one would expect from a team this large, we've conjured a *lot* of strategies for the game. ~15 of the most feasible ones are in lib/Strategies, including:

1. An algorithm that extends a connected component of our rivers towards the closest reachable disconnected mine.
2. A greedy algorithm with a variety of different cost functions.
3. An evil algorithm that finds and takes over minimal cuts between mines, so that rivals can't connect them.
4. A vicious algorithm that takes over all rivers that are directly connected to mines, so that weak unprepared rivals leave with nothing.
5. An opening algorithm that calculates and connects safe but profitable futures with corresponding mines.
6. An algorithm that starts new connected component in the best place on the map.

Naturally, we needed a way to make these strategies work together, and a way to decide which strategy works best on different maps and in different stages of the game.

[Our final bot](lib\Ai\StrategicFizzBuzz\CompositeStrategicAi.cs) utilizes a priority list of strategies. That is, it selects the best move (if any) of the highest priority strategy.
Some strategies can, of course, decide to do nothing and leave the decision to lower priority ones.

At around 2 hours before the end of the competition we've conducted a mass tournament between various combinations of our strategies on 4-player and 8-player maps.
Some of them came pretty close, some failed miserably, but in the end this was our final submission:

1. If both *options* and *futures* are available:
	1. Select a path of length ~sqrt(N of sites) that: a) includes a lot of mines; b) is highly connected (evaluated by calculating minimal cuts).
	2. Put as many *futures* as we can on this path, and put their targets as far away from mines as we can (but still on the chosen path).
	3. Try hard to connect all of these points (*futures* and corresponding mines) into one connected component.
	4. As soon as this happens or is not longer possible, continue by extending connected components towards the closest disconnected mine.
	5. As soon as all mines are connected into a single component, or it is no longer possible, take other rivers greedily.
	6. Use *options* if necessary.
	
2. If there are either no *options* or no *futures* enabled:
	1. Start a connected component from a mine that is close to other mines.
	2. Extend this component towards the closest reachable disconnected mine.
	3. If this becomes impossible, start a new component from another mine that is close to other disconnected mines. 
	4. As soon as all mines are connected into components, or it is no longer possible, take other rivers greedily.


## Members

Hey all, fill in your areas of responsibility please!

* Alexey Dubrovin
  * Offline game runner.
  * Online protocol improvements and fixes.
  * Online game runner improvement with Alexey Kirpichnikov — it uses 10 workers connected via Kafka message broker instead of only one instance on local computer.
  * Some help to other teammates.
* Alexey Kirpichnikov
  * Message broker + workers ecosystem that helped with: a) strategy parameters analysis (with Yuri Okulovsky and Timofey Yefimov); b) playing a *lot* of online games (with Alexey Dubrovin and Igor Lukanin); c) final ultimate tournament between our strategies (with Andrew Kostousov and Ivan Dashkevich).
  * FutureIsNow strategy development and improvement (with Pavel Egorov and Igor Lukanin).
* Alexey Kungurtsev
* Andrew Kostousov
* Anton Tolstov
  * Visualizer improvements: futures, splurges, auto replay.
  * Support of splurges in the protocol.
  * Development of the transport protocol at the initial stage.
* Denis Ionov
  * Game simulator implementing all the rules of the game to help us run our AIs, and select the best ones.
  * Some code to visualiser for better understanding how AIs work in the real game.
* Grigory Ovchinnikov
  * Support of futures in the protocol
  * Backend of online games statistics service
* Igor Lukanin
* Ivan Dashkevich
* Michael Khrushchev
* Michael Samoylenko
  * Some parts of the Visualizer to have some fun and relax (with Pavel Egorov)
  * Infrastructure for implementing bots via strategies to make it easier to implement new ideas and combine them with all the prior stuff.
  * MaxVertexWeight strategy to make greedier decisions based on the far-reachable sites (with Pavel Ageev)
* Pavel Ageev
* Pavel Egorov
  * Visualizer to help teammates procrastinate (and maybe debug their strategies). 
  * Saving replays from online server to Firebase db to share them with all the teammates. 
  * FutureIsNow strategy to exploit Futures option and gain 5% more scores in average. 
  * Options support to force teammates use it in their strategies.
  * BrutalTester to select the best strategy from hundreds of combinations during the last seconds of the contest.
* Stanislav Fedjanin
  * Development of online protocol and fixes
* Timofey Yefimov
  * Punters parameters optimization, attempts to make a trainable AI.
* Yuri Okulovsky

