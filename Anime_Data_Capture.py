import os
from core import *
from name_parser import name_parser
import time

def anime_lists(root, escape_folder):
    for folder in escape_folder:
        if folder in root:
            return []
    total = []
    file_type = ['.mp4', '.avi', '.rmvb', '.wmv', '.mov', '.mkv', '.flv', '.ts', '.webm', '.MP4', '.AVI', '.RMVB', '.WMV','.MOV', '.MKV', '.FLV', '.TS', '.WEBM', '.iso','.ISO']
    dirs = os.listdir(root)
    for entry in dirs:
        f = os.path.join(root, entry)
        if os.path.isdir(f):
            total += anime_lists(f, escape_folder)
        elif os.path.splitext(f)[1] in file_type:
            total.append(f)
    return total

def create_data_and_move(file_path, cookie):
    file_name, season_num, episode_num = name_parser(file_path)       # 这里的filename最好是打算直接读取出sub_id而不是动画名称，动画名称在搜索中取得的id可能不准确
    try:
        print("[!]Making Data for [{}]".format(os.path.basename(file_path)))
        core_anime(file_path, file_name, season_num, episode_num, cookie)
        print("[*]======================================================")
    except Exception as err:
        print("[-] [{}] ERROR:".format(file_path))
        print('[-]', err)


def main():
    root_path = input("请输入目标文件夹：")
    #cookie = input("请输入bgm的cookie（chii_sid）：")
    cookie = input("请输入bgm的cookie：")
    os.chdir(root_path)
    anime_list = anime_lists('.', ['output'])

    count = 0
    count_all = str(len(anime_list))
    print('[+]Find', count_all, 'anime file(s)')
    for anime_path in anime_list:
        count = count + 1
        percentage = str(count / int(count_all) * 100)[:4] + '%'
        print('[!] - ' + percentage + ' [' + str(count) + '/' + count_all + '] -')
        create_data_and_move(anime_path, cookie)
        time.sleep(5)

if __name__ == '__main__':
    main()