import json
import os
import sys

path = os.path.dirname(os.path.realpath(__file__))
ci_file = ''

def open_ci():
    with open(f'{path}/ci.json') as json_data:
        ci_file = json.load(json_data)
        json_data.close()
        return ci_file
    

def main():
    global ci_file
    ci_file = open_ci()
    beta = ''
    if len(sys.argv) == 2:
        if sys.argv[1] == 'true':
            beta = '-beta'
    print(f'{ci_file["version"]["major"]}.{ci_file["version"]["minor"]}.{ci_file["version"]["revision"]}{beta}')

if __name__ == '__main__':
    main()