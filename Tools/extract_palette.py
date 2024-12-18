from PIL import Image
import argparse
from collections import Counter

def extract_palette(image_path, num_colors):
    """
    Extract the most common colors from an image.
    
    Args:
        image_path (str): Path to the image file.
        num_colors (int): Number of top colors to extract.

    Returns:
        list: List of RGBA color tuples.
    """
    image = Image.open(image_path).convert('RGBA')
    pixels = list(image.getdata())  # Get all pixel colors
    color_counts = Counter(pixels)  # Count color frequency

    # Sort colors by frequency
    sorted_colors = sorted(color_counts.items(), key=lambda x: -x[1])
    return [color[0] for color in sorted_colors[:num_colors]]

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Extract the most common colors from an image.")
    parser.add_argument("image_path", type=str, help="Path to the image file.")
    parser.add_argument("num_colors", type=int, help="Number of top colors to extract.")
    args = parser.parse_args()

    try:
        palette = extract_palette(args.image_path, args.num_colors)
        print("Extracted Palette:")
        for i, color in enumerate(palette, start=1):
            print(f"Color {i}: {color}")
    except Exception as e:
        print(f"Error: {e}")
