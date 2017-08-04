
def pairwise_output(data, fname = 'output.csv'):

    with open(fname,'w') as file:
        for index, party in enumerate(data):
            if index == 0:
                keys = party[0].values
                file.write(",".join(keys))
                file.write(',rank\n')

            for first in range(len(party)):
                for second in range(first+1,len(party)):
                    for key in keys:
                        file.write(party[first][key]-party[second][key])
                        file.write(',')
                    file.write(party[first]['rank']-party[second]['rank'])
                    file.write('\n')




