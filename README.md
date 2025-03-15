# ChessIdentifier

This project is a Chess Board Identifier that processes a given image of a chessboard, detects the board and pieces, and provides statistics for both White and Black teams. The program identifies the position of each piece and outputs their counts and locations.

![imagem](https://github.com/user-attachments/assets/69907ef9-8f13-4013-bc21-6b186864d937)

## **Features**
- Detects and extracts the chessboard from an image.
- Identifies the pieces on the board.
- Provides a structured representation of the board.

**Outputs statistics such as:**
- Number of pieces per team (White & Black).
- Position of each piece.
- Piece Notation

## The program uses the following notation to represent the chess pieces:

**X → Empty square**

**White Pieces (Uppercase):**
- C → Knight
- T → Rook
- Q → Queen
- B → Bishop
- K → King
- P → Pawn

**Black Pieces (Lowercase):**
- c → Knight
- t → Rook
- q → Queen
- b → Bishop
- k → King
- p → Pawn

## Demonstration
- [Youtube Video](https://youtu.be/g9NelrHHkfE)

## **How?**
This program is based on image processing and pixel manipulation.

Each pixel is divided into 3 layers:
- Red, which represents the reds
- Green, which represents the greens
- Blue, which represents the blues

You can manipulate the pixels to do certain things, such as
- Rotations
- Cropping
- Contrast
- Scale
- Convert to Black and White
- Mean
- Binarization
- Histograms

## Examples:

![Captura de ecrã 2025-03-15 230508](https://github.com/user-attachments/assets/9561af79-8c6e-4d98-ab00-465a5e65f6f4)
![Captura de ecrã 2025-03-15 230520](https://github.com/user-attachments/assets/7bedd24d-4a7c-4e4d-9a2c-3c469481fa01)
![Captura de ecrã 2025-03-15 230655](https://github.com/user-attachments/assets/f4670f72-4b74-4428-bdba-fc8c426f1888)
![Captura de ecrã 2025-03-15 230645](https://github.com/user-attachments/assets/c42ede3a-c50c-48b2-a128-f31f66a45ed0)
![Captura de ecrã 2025-03-15 230707](https://github.com/user-attachments/assets/2865f7c4-7070-4ed5-94f7-af5cedd67d1f)
![Captura de ecrã 2025-03-15 230723](https://github.com/user-attachments/assets/11bc552b-f90d-4544-8e1b-616e4362587f)
