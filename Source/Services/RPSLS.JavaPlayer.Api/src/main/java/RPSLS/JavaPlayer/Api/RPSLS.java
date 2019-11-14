package RPSLS.JavaPlayer.Api;

public enum RPSLS {
	rock(0),
	paper(1),
	scissors(2),
	lizard(3),
	spock(4);

    private int value;

	private RPSLS(int value){
        this.value = value;
	}
	
    public int getValue() {
        return value;
    }
}