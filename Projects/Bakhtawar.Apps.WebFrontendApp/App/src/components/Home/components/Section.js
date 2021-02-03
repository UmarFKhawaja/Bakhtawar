import React from 'react';
import { Container, Row } from 'reactstrap';
import styled from 'styled-components';

const StyledSection = styled.section.attrs(({ id, light }) => ({
  id,
  className: light ? `bg-light` : ``
}))``;

const StyledColumn = styled.div.attrs(() => ({
  className: 'col-lg-8 mx-auto'
}))``;

export const Section = ({ id, light, children }) => {
  return (
    <StyledSection id={id} light={light}>
      <Container>
        <Row>
          <StyledColumn>
            {children}
          </StyledColumn>
        </Row>
      </Container>
    </StyledSection>
  );
};
