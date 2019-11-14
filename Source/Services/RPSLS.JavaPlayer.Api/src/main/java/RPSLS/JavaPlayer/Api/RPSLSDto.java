package RPSLS.JavaPlayer.Api;
import java.net.InetAddress;

public class RPSLSDto {
    private String text;
    private int value;
    private String player;

    public RPSLSDto(RPSLS pick) {
        this.setText(pick.name());
        this.setValue(pick.getValue());
        try {
            this.player = InetAddress.getLocalHost().getHostName();
        } catch(java.net.UnknownHostException ex) {
            this.player="_unknown_";
        }
    }

    public String getPlayer() {
        return player;
    }

    public String getPlayerType() {
        return "java";
    }

    public int getValue() {
        return value;
    }

    public void setValue(int value) {
        this.value = value;
    }

    public String getText() {
        return text;
    }

    public void setText(String text) {
        this.text = text;
    }
}