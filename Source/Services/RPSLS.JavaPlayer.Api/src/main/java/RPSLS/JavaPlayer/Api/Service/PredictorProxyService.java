package RPSLS.JavaPlayer.Api.Service;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLConnection;

import org.json.simple.JSONObject;
import org.json.simple.JSONValue;

import RPSLS.JavaPlayer.Api.RPSLS;

public class PredictorProxyService {
	private String predictorUrl;

	public PredictorProxyService(String predictorUrl) {
		this.predictorUrl = predictorUrl;
	}

	public RPSLS getPickPredicted(String username) throws MalformedURLException, IOException {
		String response = getResponseFromPredictor(predictorUrl + "&humanPlayerName=" + username);
		return getFromResponse(response);
	}

	private static String getResponseFromPredictor(String urlQueried) throws MalformedURLException, IOException {
		URL url = new URL(urlQueried);
		URLConnection conn = url.openConnection();

		BufferedReader in = new BufferedReader(new InputStreamReader(conn.getInputStream()));
		String inputLine;
		StringBuffer content = new StringBuffer();
		while ((inputLine = in.readLine()) != null) {
			content.append(inputLine);
		}
		in.close();

		return content.toString();
	}

	private static RPSLS getFromResponse(String response) {
		Object obj = JSONValue.parse(response);
		JSONObject jo = (JSONObject) obj;
		String prediction = (String) jo.get("prediction");
		return RPSLS.valueOf(prediction.toLowerCase());
	}
}