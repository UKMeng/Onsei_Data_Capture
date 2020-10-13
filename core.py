import json
import os
import re
import requests
import bangumi
import shutil
from search import get_id
from PIL import Image, ImageFilter

# config
success_folder = './output/'


def get_data_from_json(sub_id, s_num, ep_num, cookie):
    data1, data2 = bangumi.main(sub_id, ep_num, cookie)
    sub_data = json.loads(data1)
    ep_data = json.loads(data2)
    ep_data['episode'] = ep_num
    ep_data['season'] = s_num
    print(sub_data['summary'])
    print(sub_data)
    print(ep_data)
    return sub_data, ep_data

def create_folder(success_folder, sub_data, s_num):
    title = sub_data['title']
    anime_folder_path = success_folder + title + '/'
    season_folder_path = anime_folder_path + 'Season ' + str(s_num) + '/'
    if not os.path.exists(anime_folder_path):
        os.makedirs(anime_folder_path)
    if not os.path.exists(season_folder_path):
        os.makedirs(season_folder_path)
    return anime_folder_path, season_folder_path

def cut_image(path, file_name, f1, f2):
    try:
        png_name = file_name.replace(f1, f2)
        file_path = os.path.join(path, file_name)
        png_path = os.path.join(path, png_name)
        img = Image.open(file_path)
        w = img.width
        h = img.height
        img2 = img.crop((w - h / 1.5, 0, w, h))
        img2.save(png_path)
        print('[+]Image Cutted!  ' + png_path)
    except:
        print('[-]Cover cut failed!')

def check_subtitle(file_path, file_name, season_folder_path):
    file_type = ['.srt', '.ass', '.SRT', '.ASS']
    base_name = os.path.splitext(os.path.basename(file_path))[0]
    root = os.path.dirname(file_path)
    dirs = os.listdir(root)
    for entry in dirs:
        ff = os.path.join(root, entry)
        f = os.path.basename(ff)
        if os.path.splitext(f)[1] in file_type and os.path.splitext(f)[0] == base_name:
            sub_name = f
            suffix = os.path.splitext(f)[1]
            print("[+]找到字幕" + sub_name)
            new_name = file_name + suffix
            shutil.move('./'+sub_name, season_folder_path+new_name)
            print("[+]字幕已转移")
            break

    
def image_process(file_path, anime_folder_path, season_folder_path, s_num, ep_num):
    v_name = os.path.basename(file_path)
    image_name = re.sub('\..*$', '.jpg', v_name)
    file_name = 'S' + str(s_num) + 'E' + str(ep_num)
    thumb_name = file_name + '-thumb.jpg'
    fanart_name = 'fanart.jpg'
    shutil.copyfile('./' + image_name, anime_folder_path+fanart_name)
    shutil.move('./'+image_name, season_folder_path+thumb_name)
    cut_image(anime_folder_path, fanart_name, fanart_name, 'poster.jpg')
    check_subtitle(file_path, file_name, season_folder_path)#检查是否有字幕

def print_files(anime_folder_path, season_folder_path, sub_data, ep_data):
    # tvshow.nfo
    title = sub_data['title']
    studio = sub_data['studio']
    year = sub_data['year']
    outline = sub_data['summary']
    director = sub_data['director']
    tag = sub_data['tags']
    start_date = sub_data['start_date']
    end_date = sub_data['end_date'] 
    rate = sub_data['rate']
    try:
        if not os.path.exists(anime_folder_path):
            os.makedirs(anime_folder_path)
        if not os.path.exists(anime_folder_path +  "tvshow.nfo"):
            with open(anime_folder_path +  "tvshow.nfo", "wt", encoding='UTF-8') as code:
                print('<?xml version="1.0" encoding="UTF-8" ?>', file=code)
                print("<tvshow>", file=code)
                print(" <title>" + title + "</title>", file=code)
                print("  <set>", file=code)
                print("  </set>", file=code)
                print("  <studio>" + studio + "+</studio>", file=code)
                print("  <year>" + year + "</year>", file=code)
                print("  <outline>" + outline + "</outline>", file=code)
                print("  <plot>" + outline + "</plot>", file=code)
                print("  <director>" + director + "</director>", file=code)
                print("  <art>", file=code)
                print("     <poster>poster.jpg</poster>", file=code)
                print("     <fanart>fanart.jpg</fanart>", file=code)
                print("  </art>", file=code)
                print("  <maker>" + studio + "</maker>", file=code)
                try:
                    for i in tag:
                        print("  <genre>" + i + "</genre>", file=code)
                except:
                    aaaaaaaa = ''
                print("  <premiered>" + start_date + "</premiered>", file=code)
                print("  <rating>" + rate + "</rating>", file=code)
                print("  <releasedate>" + start_date + "</yreleasedate>", file = code)
                print("  <enddate>" + end_date + "</enddate>", file = code)
                print("</tvshow>", file=code)
                print("[+]Wrote!    " + anime_folder_path +  "tvshow.nfo")
    except IOError as e:
        print("[-]Write Failed!")
        print(e)
        # moveFailedFolder(filepath, failed_folder)
        return
    except Exception as e1:
        print(e1)
        print("[-]Write Failed!")
        # moveFailedFolder(filepath, failed_folder)
        return
    #### SxEx.nfo
    s_num = str(ep_data['season'])
    ep_num = str(ep_data['episode'])
    aired = ep_data['air_date']
    plot = ep_data['summary']
    runtime = str(ep_data['runtime'])
    ep_title = ep_data['title']
    file_name = 'S' + s_num + 'E' + ep_num
    try:
        if not os.path.exists(season_folder_path):
            os.makedirs(season_folder_path)
        with open(season_folder_path + file_name + ".nfo", "wt", encoding='UTF-8') as code:
            print('<?xml version="1.0" encoding="UTF-8" ?>', file=code)
            print("<episodedetails>", file=code)
            print(" <title>" + ep_title + "</title>", file=code)
            print("  <set>", file=code)
            print("  </set>", file=code)
            print("  <runtime>" + runtime + "+</runtime>", file=code)
            print("  <year>" + aired[0:4] + "</year>", file=code)
            print("  <outline>" + plot + "</outline>", file=code)
            print("  <plot>" + plot + "</plot>", file=code)
            print("  <art>", file=code)
            print("     <thumb>" + file_name + '-thumb.jpg' + "</thumb>", file=code)
            print("  </art>", file=code)
            print("  <aired>" + aired + "</aired>", file = code)
            print("  <season>" + s_num + "</season>", file = code)
            print("  <episode>" + ep_num + "</episode>", file=code)
            print("</episodedetails>", file=code)
            print("[+]Wrote!    " + season_folder_path + file_name + ".nfo")
    except IOError as e:
        print("[-]Write Failed!")
        print("[-]" + e)
        # moveFailedFolder(filepath, failed_folder)
        return
    except Exception as e1:
        print("[-]" + e1)
        print("[-]Write Failed!")
        # moveFailedFolder(filepath, failed_folder)
        return

def core_main(file_path, name, s_num, ep_num, cookie):
    if name.isdigit(): anime_id = int(name) 
    else: anime_id = get_id(name)
    sub_data, ep_data = get_data_from_json(anime_id, s_num, ep_num, cookie)             # 数据获取部分完成，接下去是文件操作部分，如创建文件夹，识别同一文件名的图片作为thumb，用ai识别接口剪辑图片成封面，重命名文件，移动文件，创建nfo文件并写入信息
    anime_folder_path, season_folder_path = create_folder(success_folder, sub_data, s_num)
    image_process(file_path, anime_folder_path, season_folder_path, s_num, ep_num)
    print_files(anime_folder_path, season_folder_path, sub_data, ep_data)
    file_rule = 'S' + str(s_num) + 'E' + str(ep_num)
    basename = os.path.basename(file_path)
    file_name = re.sub('^.*(?=\..*)', file_rule, basename)
    shutil.move(file_path, season_folder_path + file_name)

if __name__ == '__main__':
    check_subtitle('')