namespace Utils {
	public class Coordinates {
		public int x, y;
		public Coordinates(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public string ToString() {
			return "[" + this.x + "," + this.y + "]";
		}
			
	}
}