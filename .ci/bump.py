# Versioning script, it's been a while with python but this will do....ish
# The idea is that I am in control of the versioning and what bump will happen
# before the build happens...
# Yea, that didn't work out so great. Back to tagging but some slight control.
import sys
import json
import os

path = os.path.dirname(os.path.realpath(__file__))

def open_ci():
    with open(f'{path}/ci.json') as json_data:
        ci_file = json.load(json_data)
        json_data.close()
        return ci_file
    
def bump_major(ci_file, version):
    if ci_file["version_config"]["major"] == (int(version[0]) + 1):
        version[0] = int(version[0]) + 1
        print(f'{version[0]}.0.0')
    else:
        print('ERROR: CI config and version increase must match.')
        exit(1)

def bump_minor(version):
    version[1] = int(version[1]) + 1
    print(f'{version[0]}.{version[1]}.0')

def bump_revision(version):
    version[2] = int(version[2]) + 1
    print(f'{version[0]}.{version[1]}.{version[2]}')

def main():
    version = sys.stdin.read().rstrip().split('.')
    if not version or not version[0] or len(version) < 3:
        print('ERROR: Version unspecified.')
        exit(1)
    ci_file = open_ci()

    match ci_file["version_config"]["bump"]:
        case 'major':
            bump_major(ci_file, version)
        case 'minor':
            bump_minor(version)
        case 'rev':
            bump_revision(version)
        case _:
            bump_revision(version)
    

if __name__ == '__main__':
    main()