def one_out(list):
    for i in range(len(list)):
        yield {'Players' : [element for index, element in enumerate(list) if index!=i] }

