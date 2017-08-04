

def get_dummies(count, length):
    return [
            {
                'Name' : str(i),
                'Params' : { 'key_'+str(j) : i for j in range(length) }}
                    for i in range(count)  ]


def get_dummy_ai(count):
    fraction = 1.0/count
    return [
        {
            'Name' : str(i),
            'Params' :
                {
                    'Param' : (fraction*i)
                }
        }
        for i in range(count)
    ]