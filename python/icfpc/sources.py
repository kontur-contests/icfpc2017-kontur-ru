

def get_dummies(count, length):
    return [
            {
                'Name' : str(i),
                'Params' : { 'key_'+str(j) : i for j in range(length) }}
                    for i in range(count)  ]

