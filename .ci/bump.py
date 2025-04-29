# Versioning script, it's been a while with python but this will do....ish
# The idea is that I am in control of the versioning and what bump will happen
# before the build happens...
import json
import sys
import os

path = os.path.dirname(os.path.realpath(__file__))
ci_file = ''

def open_ci():
    with open(f'{path}/ci.json') as json_data:
        ci_file = json.load(json_data)
        json_data.close()
        return ci_file
    
def save_ci():
    global ci_file
    with open(f'{path}/ci.json', 'w', encoding='utf-8') as f:
        json.dump(ci_file, f, ensure_ascii=False, indent=4)
        f.close()

def bump_major():
    global ci_file
    ci_file["version"]["major"] += 1
    ci_file["version"]["minor"] = 0
    ci_file["version"]["revision"] = 0

def bump_minor():
    global ci_file
    ci_file["version"]["minor"] += 1
    ci_file["version"]["revision"] = 0

def bump_revision():
    global ci_file
    ci_file["version"]["revision"] += 1

def main():
    global ci_file
    ci_file = open_ci()
    bump = ci_file["version"]["bump"]
   
    match bump:
        case 'major':
            bump_major()
        case 'minor':
            bump_minor()
        case 'rev':
            bump_revision()
        case _:
            bump_revision()

    ci_file["version"]["bump"] = "rev"

    save_ci()
    print(f'{ci_file["version"]["major"]}.{ci_file["version"]["minor"]}.{ci_file["version"]["revision"]}')

if __name__ == '__main__':
    main()