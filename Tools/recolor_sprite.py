from PIL import Image
import argparse
import pandas as pd
import shutil
import os

def recolor_image(image, original_palette, new_palette):
    pixels = image.load()
    width, height = image.size
    
    for y in range(height):
        for x in range(width):
            current_color = pixels[x, y]
            
            if current_color in original_palette:
                index = original_palette.index(current_color)
                pixels[x, y] = new_palette[index]

def load_palettes_from_csv(csv_path):
    df = pd.read_csv(csv_path)
    palettes = {}
    
    for style in df['Style'].unique():
        palettes[style] = [tuple(map(int, color.strip('()').split(','))) for color in df[df['Style'] == style]['Color'].tolist()]
    
    return palettes

def recolor_image_with_palettes(image_path, csv_path):
    palettes = load_palettes_from_csv(csv_path)
    
    original_style_name = list(palettes.keys())[0]
    original_palette = palettes.pop(original_style_name)
    
    image = Image.open(image_path).convert('RGBA')
    
    backup_path = image_path + ".bak"
    shutil.copy(image_path, backup_path)
    print(f"Backup of the original image saved to {backup_path}")
    
    directory, original_filename = os.path.split(image_path)
    
    for style, new_palette in palettes.items():
        if original_style_name in original_filename:
            new_filename = original_filename.replace(original_style_name, style)
        else:
            filename_without_ext, ext = os.path.splitext(original_filename)
            new_filename = f"{filename_without_ext}_{style}{ext}"
        
        new_image_path = os.path.join(directory, new_filename)
        
        recolored_image = image.copy()
        recolor_image(recolored_image, original_palette, new_palette)
        
        recolored_image.save(new_image_path)
        print(f"Recolored image saved as {new_image_path}")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Recolor an image with different palettes.")
    parser.add_argument('image_path', type=str, help='Path to the image to recolor.')
    parser.add_argument('csv_path', type=str, help='Path to the CSV file containing the palettes.')

    args = parser.parse_args()

    recolor_image_with_palettes(args.image_path, args.csv_path)
