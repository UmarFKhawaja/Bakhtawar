import React from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from 'components/NavMenu';

export const Layout = ({ children }) => (
  <>
    <NavMenu/>
    <Container>
      {children}
    </Container>
  </>
);
