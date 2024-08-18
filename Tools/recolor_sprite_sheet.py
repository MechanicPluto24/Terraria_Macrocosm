from PIL import Image
import argparse
import pandas as pd
import shutil

def recolor_image(image, original_palette, new_palette, start_x, start_y, block_width, block_height):
    pixels = image.load()
    
    for y in range(start_y, start_y + block_height):
        for x in range(start_x, start_x + block_width):
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

def recolor_sprite_sheet(image_path, block_width, block_height, csv_path, layout='horizontal'):
    backup_path = image_path + ".bak"
    shutil.copy(image_path, backup_path)
    print(f"Backup of the original sprite sheet saved to {backup_path}")
    
    palettes = load_palettes_from_csv(csv_path)
    
    original_palette = palettes.pop(list(palettes.keys())[0])
    
    image = Image.open(image_path).convert('RGBA')
    image_width, image_height = image.size
    
    for i, style in enumerate(palettes.keys()):
        if layout == 'horizontal':
            start_x = (i + 1) * block_width
            start_y = 0
        else: 
            start_x = 0
            start_y = (i + 1) * block_height
        
        if start_x + block_width > image_width or start_y + block_height > image_height:
            print(f"Skipping {style}, not enough space in the image.")
            continue

        recolor_image(image, original_palette, palettes[style], start_x, start_y, block_width, block_height)

    image.save(image_path)
    print(f"Recolored sprite sheet saved and replaced the original file: {image_path}")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Recolor a sprite sheet with different palettes.")
    parser.add_argument('image_path', type=str, help='Path to the sprite sheet image.')
    parser.add_argument('block_width', type=int, help='Width of each style block.')
    parser.add_argument('block_height', type=int, help='Height of each style block.')
    parser.add_argument('csv_path', type=str, help='Path to the CSV file containing the palettes.')
    parser.add_argument('--layout', type=str, default='horizontal', choices=['horizontal', 'vertical'], help='Layout of the sprite sheet (horizontal or vertical).')

    args = parser.parse_args()

    recolor_sprite_sheet(args.image_path, args.block_width, args.block_height, args.csv_path, layout=args.layout)
