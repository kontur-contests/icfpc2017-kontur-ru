# ICFPC 2017. Team kontur.ru

## Key ideas

[Our final bot](lib\Ai\StrategicFizzBuzz\CompositeStrategicAi.cs) the priority list of strategies. 
It selects the best move (if any) of the most priority strategy. 
Yes, some strategies may return zero moves if it has nothing to do. 

Finally we had ~15 different strategies (see lib\Strategies) including:

1. Extending connectivity component towards closest reachable not our mine.
2. Greedy algorithm with with a variety of different cost functions.
3. Find and take minimal cut between mines.
4. Take all rivers, connected to the mines.
5. Connect futures with corresponding mines.
6. Start new connectivity component in the best place on the map.

At some point of the contest we had evaluated many combinations of these strategies on the 4-8 player maps.
And had found the best combination:

The final AI do the following:

1. If there are Options and Futures available, then is uses strategy:
	1. Selects path with desired length (~sqrt(N of sites)) of highly connected sites (minimal cut size used!) with a lot of mines.
	2. Put onto that way as many Futures as mines on that way already.
	3. Run strategy for connecting Futures with mines.
	4. When it completed try to Extend component towards closest reachable not our mine.
	5. If it is impossible try to take rivers greedy.
	6. Use Options if it is necessary.
	
2. If there are no Options or no Futures, then:
	1. Selects the optimal mine to start from.
	2. Try to Extend component towards closest reachable not our mine.
	3. If it is impossible try to start new component from the best not our mine.
	4. If it is impossible try to take rivers greedy.


## Members

Hey all, fill in your areas of responsibility please!

* Alexey Dubrovin
* Alexey Kirpichnikov
* Alexey Kungurtsev
* Andrew Kostousov
* Anton Tolstov. Visualizer improvements: futures, splurges, auto replay. Support splurges in the protocol.
Development of the transport protocol at the initial stage.
* Denis Ionov
* Grigory Ovchinnikov
* Igor Lukanin
* Ivan Dashkevich
* Michael Khrushchev
* Michael Samoylenko
* Pavel Ageev
* Pavel Egorov. Visualizer to help teammates procrastinate (and maybe debug their strategies). 
Saving replays from online server to Firebase db to share them with all the teammates. 
FutureIsNow strategy to exploit Futures option and gain 5% more scores in average. 
Options support to force teammates use it in their strategies, 
BrutalTester to select the best strategy from hundreds of combinations during the last seconds of the contest.
* Stanislav Fedjanin
* Timofey Yefimov
* Yuri Okulovsky

