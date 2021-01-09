import os
import re
import shutil
from core import core_voice
from name_parser import name_parser

def voice_lists(root, escape_folder):
    for folder in escape_folder:
        if folder in root:
            return []
    total = []
    dirs = os.listdir(root)
    print(dirs)
    for entry in dirs:
        dlid = entry.upper()
        f = os.path.join(root, entry)
        if os.path.isdir(f):
            if(re.match('(RJ|VJ)\d+', dlid)):
                total.append(f)
    return total

def create_data_and_move(file_path):
    dlid = os.path.basename(file_path).upper()
    try:
        print("[!]Making Data for [{}]".format(os.path.basename(file_path)))
        core_voice(file_path, dlid)
        print("[*]======================================================")
    except Exception as err:
        print("[-] [{}] ERROR:".format(file_path))
        print('[-]', err)


def main():
    root_path = input("请输入目标文件夹：")
    shutil.copy("./extra/补充封面.mp3", root_path)
    os.chdir(root_path)
    voice_list = voice_lists('.', ['output'])  # 按照文件夹识别
    count = 0
    count_all = str(len(voice_list))
    print('[+]Find', count_all, 'voice album(s)')
    for voice_path in voice_list:
        count = count + 1
        percentage = str(count / int(count_all) * 100)[:4] + '%'   
        print('[!] - ' + percentage + ' [' + str(count) + '/' + count_all + '] -')    # 进度条
        create_data_and_move(voice_path)
    os.remove("./补充封面.mp3")

if __name__ == '__main__':
    main()