
def pairwise_output(data, fname = 'output.csv'):

    with open(fname,'w') as file:
        for index, party in enumerate(data):
            if index == 0:
                keys = party['Players'][0]['Params']
                file.write(",".join(keys))
                file.write(',rank\n')
            players = party['Players']
            for first in range(len(players)):
                for second in range(first+1,len(players)):
                    for key in keys:
                        file.write(str(players[first]['Params'][key]-players[second]['Params'][key]))
                        file.write(',')
                    file.write(str(players[first]['Rank']-players[second]['Rank']))
                    file.write('\n')




