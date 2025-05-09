import requests
from PIL import Image, ImageDraw, ImageColor
from io import BytesIO
import pycountry

def trim_transparent(image):
    bbox = image.getbbox()
    if bbox:
        trimmed_image = image.crop(bbox)
        return trimmed_image
    else:
        return image
    
def generate_flag(country_code, style, size, output_file, canvas_size=(0, 0), offset=(0, 0), trim = False):
    flag_url = f"https://flagsapi.com/{country_code}/{style}/{size}.png"

    response = requests.get(flag_url)

    if response.status_code != 200:
        print(f"Failed to fetch flag image for {country_code}")
        return (False, 0, 0)

    flag = Image.open(BytesIO(response.content))
    flag = trim_transparent(flag)   

    if(canvas_size[0] == 0 or canvas_size[1] == 0):
        canvas_size = (flag.width, flag.height)

    canvas = Image.new('RGBA', canvas_size, (255, 255, 255, 0))
    position = (offset[0], offset[1])
    canvas.paste(flag, position)

    if(trim):
        canvas = trim_transparent(canvas)

    canvas.save(output_file)
    return (True, canvas_size[0], canvas_size[1])

def generate_all_flags(size, output_folder, canvas_size=(0, 0), offset=(0, 0), prefix = '', trim = False):
    for country in pycountry.countries:
        output_file = f"{output_folder}/{prefix}{country.alpha_3}.png"
        (result, width, height) = generate_flag(country.alpha_2, 'flat', size, output_file, canvas_size, offset, trim)
        if result:
            print(f"Generated {width}x{height} flag for {country.name} - {country.alpha_3}")

generate_all_flags(32, './Icons/', canvas_size=(30, 20), offset=(0, 0), prefix="Flag_", trim = True)
generate_all_flags(32, './Decals/', canvas_size=(120, 302), offset=(45, 18), prefix="Flag_", trim = False)

