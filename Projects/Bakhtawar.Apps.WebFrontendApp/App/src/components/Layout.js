import React from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';

export const Layout = ({ children }) => (
  <>
    <NavMenu/>
    <Container>
      {children}
    </Container>
  </>
);
