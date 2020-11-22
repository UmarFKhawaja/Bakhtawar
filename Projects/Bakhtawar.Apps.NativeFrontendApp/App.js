import React, { useState, useCallback, useMemo, useEffect } from 'react';
import { Alert } from 'react-native';
import { authorize, refresh, revoke, prefetchConfiguration } from 'react-native-app-auth';
import { Page, Button, ButtonContainer, Form, FormLabel, FormValue, Heading } from './components';

const configs = {
  bakhtawar: {
    issuer: 'https://id.bakhtawar.co.uk',
    clientId: 'bakhtawar.app',
    redirectUrl: 'uk.co.bakhtawar.id:/oauthredirect',
    additionalParameters: {},
    scopes: ['openid', 'profile', 'email']
  },
  google: {
    issuer: 'https://accounts.google.com',
    clientId: '571139809677-n4e82q67c7pj1n3ut4uueef93gerktig.apps.googleusercontent.com',
    redirectUrl: 'com.googleusercontent.apps.571139809677-n4e82q67c7pj1n3ut4uueef93gerktig:/oauth2redirect/google',
    additionalParameters: {},
    scopes: ['openid', 'profile']
  },
  identityserver: {
    issuer: 'https://demo.identityserver.io',
    clientId: 'interactive.public',
    redirectUrl: 'io.identityserver.demo:/oauthredirect',
    additionalParameters: {},
    scopes: ['openid', 'profile', 'email', 'offline_access']
    // serviceConfiguration: {
    //   authorizationEndpoint: 'https://demo.identityserver.io/connect/authorize',
    //   tokenEndpoint: 'https://demo.identityserver.io/connect/token',
    //   revocationEndpoint: 'https://demo.identityserver.io/connect/revoke'
    // }
  }
};

const defaultAuthState = {
  hasLoggedInOnce: false,
  provider: '',
  accessToken: '',
  accessTokenExpirationDate: '',
  refreshToken: ''
};

const App = () => {
  const [authState, setAuthState] = useState(defaultAuthState);
  useEffect(() => {
    prefetchConfiguration({
      warmAndPrefetchChrome: true,
      ...configs.identityserver
    }).then();
  }, []);

  const handleAuthorize = useCallback(
    async (provider) => {
      try {
        const config = configs[provider];
        const newAuthState = await authorize(config);

        setAuthState({
          hasLoggedInOnce: true,
          provider: provider,
          ...newAuthState
        });
      } catch (error) {
        Alert.alert('Failed to log in', error.message);
      }
    },
    []
  );

  const handleRefresh = useCallback(async () => {
    try {
      const config = configs[authState.provider];
      const newAuthState = await refresh(config, {
        refreshToken: authState.refreshToken
      });

      setAuthState((current) => ({
        ...current,
        ...newAuthState,
        refreshToken: newAuthState.refreshToken || current.refreshToken
      }));

    } catch (error) {
      Alert.alert('Failed to refresh token', error.message);
    }
  }, [authState]);

  const handleRevoke = useCallback(async () => {
    try {
      const config = configs[authState.provider];
      await revoke(config, {
        tokenToRevoke: authState.accessToken,
        sendClientId: true
      });

      setAuthState({
        provider: '',
        accessToken: '',
        accessTokenExpirationDate: '',
        refreshToken: ''
      });
    } catch (error) {
      Alert.alert('Failed to revoke token', error.message);
    }
  }, [authState]);

  const showRevoke = useMemo(() => {
    if (authState.accessToken) {
      const config = configs[authState.provider];
      if (config.issuer || config.serviceConfiguration.revocationEndpoint) {
        return true;
      }
    }
    return false;
  }, [authState]);

  return (
    <Page>
      {authState.accessToken ? (
        <Form>
          <FormLabel>accessToken</FormLabel>
          <FormValue>{authState.accessToken}</FormValue>
          <FormLabel>accessTokenExpirationDate</FormLabel>
          <FormValue>{authState.accessTokenExpirationDate}</FormValue>
          <FormLabel>refreshToken</FormLabel>
          <FormValue>{authState.refreshToken}</FormValue>
          <FormLabel>scopes</FormLabel>
          <FormValue>{authState.scopes.join(', ')}</FormValue>
        </Form>
      ) : (
        <Heading>{authState.hasLoggedInOnce ? 'Goodbye.' : 'Hello, stranger.'}</Heading>
      )}

      <ButtonContainer>
        {!authState.accessToken ? (
          <Button
            onPress={() => handleAuthorize('bakhtawar')}
            text="Authorize"
            color="#DA2536"
          />
        ) : null}
        {authState.refreshToken ? (
          <Button onPress={handleRefresh} text="Refresh" color="#24C2CB"/>
        ) : null}
        {showRevoke ? (
          <Button onPress={handleRevoke} text="Revoke" color="#EF525B"/>
        ) : null}
      </ButtonContainer>
    </Page>
  );
};

export default App;
