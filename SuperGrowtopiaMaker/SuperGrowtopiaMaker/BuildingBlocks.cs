using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SuperGrowtopiaMaker
{
    enum DrawPurpose
    {
        Show, Tile
    }

    class BuildingBlocks
    {
        public static Rectangle GetRectangleForTile(int id, DrawPurpose drawPurpose, int sTileID_below, int sTileID_above, int sTileID_left, int sTileID_right, int AnimationStatus)
        {
            if (drawPurpose == DrawPurpose.Tile)
            {
                switch (id)
                {
                    case 0:// Blank
                        return new Rectangle(31 * 32, 25 * 32, 32, 32);
                    case 2:// Dirt
                        if (sTileID_above != 2)
                        {
                            return new Rectangle(9 * 32, 20 * 32, 32, 32);
                        }
                        else
                        {
                            return new Rectangle(8 * 32, 20 * 32, 32, 32);
                        }
                    case 4:// Lava
                        return new Rectangle(4 * 32, 6 * 32, 32, 32);
                    case 6:// Main Door
                        return new Rectangle(17 * 32, 1 * 32, 32, 32);
                    case 8:// Bedrock
                        if (sTileID_above != 8)
                        {
                            return new Rectangle(1 * 32, 26 * 32, 32, 32);
                        }
                        else
                        {
                            return new Rectangle(0 * 32, 26 * 32, 32, 32);
                        }
                    case 10:// Rock
                        return new Rectangle(15 * 32, 1 * 32, 32, 32);
                    case 12:// Door
                        return new Rectangle(14 * 32, 1 * 32, 32, 32);
                    case 14:// Cave Background
                        return new Rectangle(24 * 32, 26 * 32, 32, 32);
                    case 16:// Grass
                        if (sTileID_left ==16 && sTileID_right ==16)
                        {
                            return new Rectangle(1 * 32, 6 * 32, 32, 32);
                        }
                        else if (sTileID_left !=16 && sTileID_right == 16)
                        {
                            return new Rectangle(0 * 32, 6 * 32, 32, 32);
                        }
                        else if (sTileID_left ==16 && sTileID_right != 16)
                        {
                            return new Rectangle(2 * 32, 6 * 32, 32, 32);
                        }
                        else
                        {
                            return new Rectangle(3 * 32, 6 * 32, 32, 32);
                        }
                    case 20:// Sign
                        return new Rectangle(21 * 32, 0 * 32, 32, 32);
                    case 22:// Daisy
                        return new Rectangle(3 * 32, 1 * 32, 32, 32);
                    case 24:// Pointy Sign
                        return new Rectangle(22 * 32, 0 * 32, 32, 32);
                    case 26:// Crappy Sign
                        return new Rectangle(24 * 32, 0 * 32, 32, 32);
                    case 28:// Danger Sign
                        return new Rectangle(23 * 32, 0 * 32, 32, 32);
                    case 30:// Dungeon Door
                        return new Rectangle(19 * 32, 1 * 32, 32, 32);
                    //case 3:// Dirt Top
                    //    return new Rectangle(9 * 32, 20 * 32, 32, 32);
                    case 32:// Lava Cube
                        return new Rectangle(10 * 32, 1 * 32, 32, 32);
                    case 52:// Wooden Background
                        return new Rectangle(0 * 32, 20 * 32, 32, 32);
                    case 102:// Wooden Platform
                        if (sTileID_left == 102 && sTileID_right == 102)
                        {
                            return new Rectangle(1 * 32, 8* 32, 32, 32);
                        }
                        else if (sTileID_left != 102 && sTileID_right == 102)
                        {
                            return new Rectangle(0 * 32, 8 * 32, 32, 32);
                        }
                        else if (sTileID_left == 102 && sTileID_right != 102)
                        {
                            return new Rectangle(2 * 32, 8 * 32, 32, 32);
                        }
                        else
                        {
                            return new Rectangle(3 * 32, 8 * 32, 32, 32);
                        }
                    case 116:// Bricks
                        return new Rectangle(8 * 32, 26 * 32, 32, 32);
                    case 162:// Death Spikes
                        if (sTileID_below != 0 && sTileID_below != 162)
                        {
                            return new Rectangle(28 * 32, 0 * 32, 32, 32);
                        }
                        else
                        {
                            if (sTileID_left != 0 && sTileID_left != 162)
                            {
                                return new Rectangle(25 * 32, 0 * 32, 32, 32);
                            }
                            else
                            {
                                if (sTileID_above != 0 && sTileID_above != 162)
                                {
                                    return new Rectangle(26 * 32, 0 * 32, 32, 32);
                                }
                                else
                                {
                                    if (sTileID_right != 0 && sTileID_right != 162)
                                    {
                                        return new Rectangle(27 * 32, 0 * 32, 32, 32);
                                    }
                                    else
                                    {
                                        return new Rectangle(29 * 32, 0 * 32, 32, 32);
                                    }
                                }
                            }
                        }
                    case 202:// Small Lock
                        return new Rectangle(3 * 32, 9 * 32, 32, 32);
                    case 204:// Big Lock
                        return new Rectangle(3 * 32, 10 * 32, 32, 32);
                    case 206:// Huge Lock
                        return new Rectangle(3 * 32, 11 * 32, 32, 32);
                    case 222:
                        if (sTileID_left == 222 && sTileID_right == 222)
                        {
                            return new Rectangle(1 * 32, 7 * 32, 32, 32);
                        }
                        else if (sTileID_left != 222 && sTileID_right == 222)
                        {
                            return new Rectangle(0 * 32, 7 * 32, 32, 32);
                        }
                        else if (sTileID_left == 222 && sTileID_right != 222)
                        {
                            return new Rectangle(2 * 32, 7 * 32, 32, 32);
                        }
                        else
                        {
                            return new Rectangle(3 * 32, 7 * 32, 32, 32);
                        }
                    case 226:// Signal Jammer
                        return new Rectangle(AnimationStatus * 32, 5 * 32, 32, 32);
                    case 242:// World Lock
                        return new Rectangle(3 * 32, 12 * 32, 32, 32);
                    case 258:// Unused Crystal Block
                        return new Rectangle(6 * 32, 3 * 32, 32, 32);

                        //return new Rectangle(29 * 32, 0 * 32, 32, 32);
                    //case 1:// Bedrock Top
                    //    return new Rectangle(1 * 32, 26 * 32, 32, 32);
                    //return new Rectangle(817, 465, 1, 1);
                }
            }

            switch (id)
            {
                case 0:// Blank
                    return new Rectangle(11 * 32, 1 * 32, 32, 32);
                case 2:// Dirt
                    return new Rectangle(12 * 32, 21 * 32, 32, 32);
                case 4:// Lava
                    return new Rectangle(8 * 32, 7 * 32, 32, 32);
                case 6:// Main Door (Mover)
                    return new Rectangle(60 * 32, 13 * 32, 32, 32);
                case 10:// Rock
                    return new Rectangle(15 * 32, 1 * 32, 32, 32);
                case 12:// Door
                    return new Rectangle(14 * 32, 1 * 32, 32, 32);
                case 14:// Cave Background
                    return new Rectangle(28 * 32, 27 * 32, 32, 32);
                case 16:// Grass
                    return new Rectangle(3 * 32, 6 * 32, 32, 32);
                case 18:
                    return new Rectangle(15 * 32, 31 * 32, 32, 32);
                case 20:// Sign
                    return new Rectangle(21 * 32, 0 * 32, 32, 32);
                case 22:// Daisy
                    return new Rectangle(3 * 32, 1 * 32, 32, 32);
                case 24:// Pointy Sign
                    return new Rectangle(22 * 32, 0 * 32, 32, 32);
                case 26:// Crappy Sign
                    return new Rectangle(24 * 32, 0 * 32, 32, 32);
                case 28:// Danger Sign
                    return new Rectangle(23 * 32, 0 * 32, 32, 32);
                case 30:// Dungeon Door
                    return new Rectangle(19 * 32, 1 * 32, 32, 32);
                //case 3:// Dirt Top
                    //return new Rectangle(9 * 32, 20 * 32, 32, 32);
                case 32:// Lava Cube
                    return new Rectangle(10 * 32, 1 * 32, 32, 32);
                case 52:// Wooden Background
                    return new Rectangle(4 * 32, 21 * 32, 32, 32);
                case 54:// Window
                    return new Rectangle(7 * 32, 1 * 32, 32, 32);
                case 56:// Glasspane
                    return new Rectangle(8 * 32, 1 * 32, 32, 32);
                case 58:// Wooden Window
                    return new Rectangle(9 * 32, 1 * 32, 32, 32);
                case 60:// Portcullis
                    return new Rectangle(14 * 32, 0 * 32, 32, 32);
                case 62:// Boombox
                    return new Rectangle(AnimationStatus * 32, 1 * 32, 32, 32);
                case 64:// Olde Timey Radio
                    return new Rectangle(AnimationStatus * 32, 2 * 32, 32, 32);
                case 100:// Wood Block
                    return new Rectangle(12 * 32, 1 * 32, 32, 32);
                case 102:// Wood Platform
                    return new Rectangle(3 * 32, 8 * 32, 32, 32);
                case 104:// Rock Background
                    return new Rectangle(16 * 32, 26 * 32, 32, 32);
                case 106:// Toilet
                    return new Rectangle(30 * 32, 0 * 32, 32, 32);
                case 108:// Yerfdog Painting
                    return new Rectangle(18 * 32, 0 * 32, 32, 32);
                case 110:// Dink Duck painting
                    return new Rectangle(19 * 32, 0 * 32, 32, 32);
                case 112:// Gems
                    return new Rectangle(22 * 32, 1 * 32, 32, 32);
                case 114:// Disco Ball
                    return new Rectangle(0 * 32, 3 * 32, 32, 32);
                case 116:// Bricks
                    return new Rectangle(12 * 32, 27 * 32, 32, 32);
                case 118:// Brick Background
                    return new Rectangle(13 * 32, 1 * 32, 32, 32);
                case 120:// Mystery Block
                    return new Rectangle(21 * 32, 1 * 32, 32, 32);
                case 162:// Death Spikes
                    return new Rectangle(29 * 32, 0 * 32, 32, 32);
                case 164:// Grey Block
                    return new Rectangle(2 * 32, 0 * 32, 32, 32);
                case 166:// Black Block
                    return new Rectangle(10 * 32, 0 * 32, 32, 32);
                case 168:// White Block
                    return new Rectangle(11 * 32, 0 * 32, 32, 32);
                case 170:// Red Block
                    return new Rectangle(3 * 32, 0 * 32, 32, 32);
                case 172:// orange Block
                    return new Rectangle(4 * 32, 0 * 32, 32, 32);
                case 174:// Yellow Block
                    return new Rectangle(5 * 32, 0 * 32, 32, 32);
                case 176:// Green Block
                    return new Rectangle(6 * 32, 0 * 32, 32, 32);
                case 178:// Aqua Block
                    return new Rectangle(7 * 32, 0 * 32, 32, 32);
                case 180:// Blue Block
                    return new Rectangle(8 * 32, 0 * 32, 32, 32);
                case 182:// Purple Block
                    return new Rectangle(9 * 32, 0 * 32, 32, 32);
                case 184:// Brown Block
                    return new Rectangle(12 * 32, 0 * 32, 32, 32);
                case 186:// Steel Block
                    return new Rectangle(13 * 32, 0 * 32, 32, 32);
                case 188:// Poppy
                    return new Rectangle(5 * 32, 1 * 32, 32, 32);
                case 190:// Rose
                    return new Rectangle(6 * 32, 1 * 32, 32, 32);
                case 192:// Bush
                    return new Rectangle(4 * 32, 1 * 32, 32, 32);
                case 194:// Mushroom
                    return new Rectangle(3 * 32, 2 * 32, 32, 32);
                case 198:// Flowery Wallpaper
                    return new Rectangle(0 * 32, 0 * 32, 32, 32);
                case 200:// Stripey Wallpaper
                    return new Rectangle(1 * 32, 0 * 32, 32, 32);
                case 202:// Small Lock
                    return new Rectangle(0 * 32, 9 * 32, 32, 32);
                case 204:// Big Lock
                    return new Rectangle(0 * 32, 10 * 32, 32, 32);
                case 206:// Huge Lock
                    return new Rectangle(0 * 32, 11 * 32, 32, 32);
                case 218:// Wooden Chair
                    return new Rectangle(18 * 32, 1 * 32, 32, 32);
                case 220:// Note Block
                    return new Rectangle(AnimationStatus * 32, 4 * 32, 32, 32);
                case 222:// Wooden Table
                    return new Rectangle(3 * 32, 7 * 32, 32, 32);
                case 224:// House Entrance
                    return new Rectangle(16 * 32, 0 * 32, 32, 32);
                case 226:// Signal Jammer
                    return new Rectangle(0 * 32, 5 * 32, 32, 32);
                case 230:// Bathtub
                    return new Rectangle(27 * 32, 1 * 32, 32, 32);
                case 242:// World Lock
                    return new Rectangle(0 * 32, 12 * 32, 32, 32);
                case 246:// Music Box
                    return new Rectangle((3 + AnimationStatus) * 32, 3 * 32, 32, 32);
                case 248:// Evil Bricks
                    return new Rectangle(((AnimationStatus == 0) ? 29 : 28 + AnimationStatus) * 32, 1 * 32, 32, 32);
                case 252:// Cuzco Wall Mount
                    return new Rectangle(20 * 32, 0 * 32, 32, 32);
                case 254:// Robot Wants Dub Step
                    return new Rectangle((3 + AnimationStatus) * 32, 4 * 32, 32, 32);
                case 258:// Unused Crystal Block
                    return new Rectangle(15 * 32, 3 * 32, 32, 32);
                case 260:// Golden Block
                    return new Rectangle(7 * 32, 2 * 32, 32, 32);
                case 262:// Crystal Block
                    return new Rectangle(7 * 32, 3 * 32, 32, 32);
                case 2966:// Enchanted Spatula
                    return new Rectangle(30 * 32, 39 * 32, 32, 32);
                default:
                    return new Rectangle(14 * 32, 2 * 32, 1, 1);
                //return new Rectangle(817, 465, 1, 1);
            }
        }
    }
}
