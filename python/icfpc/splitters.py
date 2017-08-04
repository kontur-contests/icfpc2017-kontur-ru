def one_out(list):
    for i in range(len(list)):
        yield [element for index, element in enumerate(list) if index!=i]

