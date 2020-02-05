package com.wcjung.sample;

import java.util.Arrays;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.apache.commons.codec.binary.Base64;
import org.apache.commons.lang3.StringUtils;
import org.junit.Test;
import org.springframework.http.HttpEntity;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpMethod;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.client.RestTemplate;

public class PasswordGrantTypeTest {

	@Test
	public void testPasswordGrantType(){
		String username = "username";
		String password = "password";
		
		String accessToken = requestAccessToken(username, password);
		String userInfo = requestUserInfo(accessToken);
		
		System.out.println(getJsonValue(userInfo, "userName"));
	}
	
	/**
	 * 
	 * WCJUNG 
	 * desc   :  Access Token을 요청합니다. - Password Grant Type
	 * @param  
	 * @return String
	 */
	public String requestAccessToken(String username, String password) {
        String clientId = "client_id";
		String clientSecret = "client_secret";
		
		String credentials = clientId + ":" + clientSecret;
		String encodedCredentials = new String(Base64.encodeBase64(credentials.getBytes()));
		
		String authUri = "http://oauth.sample.com/oauth/token";
		String param = "?username=" + username + "&password=" + password + "&grant_type=password&scope=ReadProfile";
		
		String httpResponse = requestOAuthService(authUri, HttpMethod.POST, "application/x-www-form-urlencoded", encodedCredentials, param);
		
		return getJsonValue(httpResponse, "access_token");
	}
    
    /**
     * 
     * WCJUNG
     * desc : JSON 문자열에서 원하는 값을 추출합니다.
     * @param source 전체 JSON 문자열
     * @param name name string
     * @return value string
     */
	private String getJsonValue(String source, String name) {
		String value = "";
		
		try {
			String regex = "\"("+ name + ")\":\"?([가-힣\\w\\-_@\\.]+)\"?";
			Pattern pattern = Pattern.compile(regex); 
			Matcher matcher = pattern.matcher(source);
		
			while (matcher.find() && value == "") {
			    value = matcher.group(2);
			}

		} catch (Exception e) {
			e.printStackTrace();
		}
		
		return value;
	}
	
	/**
	 * 
	 * WCJUNG
	 * desc   :  사용자 정보를 조회합니다.
	 * @param  
	 * @return String : Access Token
	 */
	public String requestUserInfo(UserTokenVO userToken) {
		String userInfoUri = "http://oauth.sample.com/oauth/userinfo";
		String param = "?access_token=" + userToken.getAccess_token();
		String httpResponse = requestOAuthService(userInfoUri, HttpMethod.GET, null, null, param);
				
		return httpResponse;
	}
	
	/**
	 * 
	 * WCJUNG
	 * desc   :  Http Request Method
	 * @param  
	 * @return String
	 */
	public String requestOAuthService(String uri, HttpMethod httpMethod, String userAgent, String credentials, String param) {
		RestTemplate restTemplate = new RestTemplate();
		
		HttpHeaders headers = new HttpHeaders();
		if(StringUtils.isNotBlank(userAgent)) {
			headers.setAccept(Arrays.asList(MediaType.APPLICATION_JSON_UTF8));
			headers.add("User-Agent", userAgent);
		}
		if(StringUtils.isNotBlank(credentials)) {
			headers.add("Authorization", "Basic " + credentials);
		}

		HttpEntity<String> request = new HttpEntity<String>(headers);
		String requestUrl = uri + param;

		ResponseEntity<String> response = restTemplate.exchange(requestUrl, httpMethod, request, String.class);

		String httpResponse = (null == response) ? "" : response.getBody();

		return httpResponse;
	}
}
