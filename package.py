import os
import re
import shutil
import json
import core
from WebCrawler import dlsite

# config
success_folder = './output/'

def voice_lists(root, escape_folder):
    for folder in escape_folder:
        if folder in root:
            return []
    total = []
    file_type = ['.rar', '.RAR', '.zip', '.ZIP', '.7z', '.7Z']
    dirs = os.listdir(root)
    print(dirs)
    for entry in dirs:
        f = os.path.join(root, entry)
        if os.path.isdir(f):
            total += voice_lists(f, escape_folder)
        elif os.path.splitext(f)[1] in file_type:
            total.append(f)
    return total


def create_data_and_move(file_path):
    dlid = os.path.splitext(os.path.basename(file_path))[0].upper()
    try:
        print("[!]Making Data for [{}]".format(os.path.basename(file_path)))
        voice_data = core.get_voice_data_from_json(dlid)
        album_folder_path = core.create_voice_folder(success_folder, voice_data, dlid, 1)
        shutil.move(file_path, album_folder_path)
        print("[*]======================================================")
    except Exception as err:
        print("[-] [{}] ERROR:".format(file_path))
        print('[-]', err)

def main():
    root_path = input("请输入目标文件夹：")
    os.chdir(root_path)
    voice_list = voice_lists('.', ['output'])  # 按照文件夹识别
    count = 0
    count_all = str(len(voice_list))
    print('[+]Find', count_all, 'voice packages(s)')
    for voice_path in voice_list:
        count = count + 1
        percentage = str(count / int(count_all) * 100)[:4] + '%'   
        print('[!] - ' + percentage + ' [' + str(count) + '/' + count_all + '] -')    # 进度条
        create_data_and_move(voice_path)

if __name__ == '__main__':
    main()