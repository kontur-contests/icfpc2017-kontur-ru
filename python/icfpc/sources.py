

def get_dummies(count, length):
    return [ { 'name' : str(i), 'values' : { 'key_'+str(j) : i for j in range(length) }} for i in range(count) ]


